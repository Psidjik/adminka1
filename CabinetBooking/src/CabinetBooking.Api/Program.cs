using System.Diagnostics;
using CabinetBooking;
using CabinetBooking.Api;
using CabinetBooking.Application;
using CabinetBooking.Application.Helpers;
using CabinetBooking.Domain.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Prometheus;

const int GrpcPort = 28710;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CabinetBookingDbContext>(
    options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("CabinetBookingServiceConnection")
    )
);

// Добавление gRPC сервисов
builder.Services.AddGrpc();
builder.Services.AddScoped<CabinetBookingService.CabinetBookingServiceBase, CabinetBookingGrpcService>();

builder.Services.AddScoped<IGenerateTestData, GenerateTestData>();

builder.Services.AddApplication();

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
    options.ListenAnyIP(GrpcPort, opt => opt.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

// Prometheus metrics
app.UseMetricServer();
app.UseHttpMetrics();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CabinetBookingDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// bind gRPC endpoints
app.MapWhen(ctx => ctx.Request.Host.Port == GrpcPort, intApp =>
{
    intApp.UseRouting();
    intApp.UseEndpoints(endpoints => endpoints.MapGrpcService<CabinetBookingService.CabinetBookingServiceBase>());
});

using (var scope = app.Services.CreateScope())
{
    var testDataService = scope.ServiceProvider.GetRequiredService<IGenerateTestData>();
    await testDataService.GenerateBookingTestData();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CabinetBookingDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.MapGrpcService<CabinetBookingGrpcService>();

// Prometheus metrics endpoint
app.MapMetrics("/metrics");

app.Run();