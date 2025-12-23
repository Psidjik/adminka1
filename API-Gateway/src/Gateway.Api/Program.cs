using System.Diagnostics;
using Auth;
using CabinetBooking;
using Gateway.Api;
using Gateway.Api.Schema;
using Gateway.Api.Schema.Types;
using Gateway.Api.Schema.Types.Authentication;
using Gateway.Api.Schema.Types.CabinetBooking;
using Grpc.Net.Client;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Prometheus;

const int ApiPort = 8080;
const string GraphQLHttpUrl = "/graphql/http";
const string CorsPolicy = "AllowAll";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://10.242.184.80:8080", "http://10.242.184.127:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<IMapper, Mapper>();


builder.Services.AddSingleton(services =>
{
    var cabinetBookingAddress = builder.Configuration["GrpcServices:CabinetBooking"] ?? "http://cabinet-booking:28710";
    var channel = GrpcChannel.ForAddress(cabinetBookingAddress);
    return new CabinetBookingService.CabinetBookingServiceClient(channel);
});
builder.Services.AddSingleton(services =>
{
    var userAddress = builder.Configuration["GrpcServices:User"] ?? "http://user:28711";
    var channel = GrpcChannel.ForAddress(userAddress);
    return new AuthService.AuthServiceClient(channel);
});

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false; 
    options.ListenAnyIP(ApiPort, opt => opt.Protocols = HttpProtocols.Http1AndHttp2); 
});

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>() 
    .AddTypeExtension<CabinetBookingQuery>()
    .AddMutationType<Mutation>() 
    .AddTypeExtension<CabinetBookingMutation>()
    .AddTypeExtension<AuthenticationMutation>()
    .AllowIntrospection(true)
    .InitializeOnStartup(); 

var app = builder.Build();

// Prometheus metrics
app.UseMetricServer();
app.UseHttpMetrics();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();

app.MapGraphQLHttp(GraphQLHttpUrl)
    .AllowAnonymous() 
    .WithOptions(new GraphQLHttpOptions
    {
        EnableGetRequests = false, 
    });

app.MapGraphQL();

// Настройка дополнительных endpoint'ов
app.UseWhen(ctx => ctx.Request.Host.Port == ApiPort, intApp =>
{
    intApp.UseEndpoints(endpoints =>
    {
        endpoints.MapGraphQLSchema("/graphql/schema").AllowAnonymous();

        // Endpoint для GraphQL UI (Nitro)
        endpoints.MapNitroApp("/graphql/ui").WithOptions(new GraphQLToolOptions
        {
            Title = "Nitro - Platform",
            GraphQLEndpoint = GraphQLHttpUrl
        });

        // Простой endpoint для проверки работоспособности
        endpoints.MapGet("/check", () => "locked and loaded").AllowAnonymous().ExcludeFromDescription();
        
        // Prometheus metrics endpoint
        endpoints.MapMetrics("/metrics").AllowAnonymous();
    });
});
app.Run();
