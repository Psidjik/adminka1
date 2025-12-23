using System.Text;
using Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using User;
using User.Api.Services;
using User.Application;
using User.Application.Command;
using User.Domain.Data;

const int GrpcPort = 28711;
const int MetricsPort = 28713; // HTTP порт для метрик

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDbContext>(
    options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("UserServiceConnection")
    )
);

builder.Services.AddApplication();

builder.Services.AddScoped<IJwtService, JwtService>();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddScoped<AuthService.AuthServiceBase, UserGrpcService>();

// Настройка аутентификации
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtProperties:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtProperties:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtProperties:TOKEN"])),
            ValidateIssuerSigningKey = true
        };
    });

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
    options.ListenAnyIP(GrpcPort, opt => opt.Protocols = HttpProtocols.Http2);
    options.ListenAnyIP(MetricsPort, opt => opt.Protocols = HttpProtocols.Http1); // HTTP/1.1 для метрик
});

var app = builder.Build();

// Prometheus metrics
app.UseMetricServer();
app.UseHttpMetrics();

app.MapGrpcService<UserGrpcService>();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Prometheus metrics endpoint на HTTP порту
app.MapWhen(ctx => ctx.Request.Host.Port == MetricsPort, intApp =>
{
    intApp.UseRouting();
    intApp.UseEndpoints(endpoints => endpoints.MapMetrics("/metrics"));
});

app.Run();