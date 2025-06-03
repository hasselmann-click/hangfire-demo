using Hangfire;
using Common.Hangfire;
using Common.Config;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);
builder.Services.AddHangfire((sp, config) =>
{
    var appsettings = builder.Configuration.Get<AppsettingsDto>() ?? throw new Exception("Could not load appsettings");
    ServiceRegistry.AddCustomHangfire(appsettings, config, sp);
});

builder.AddServiceDefaults();

var app = builder.Build();

app.UseHangfireDashboard(""); // map to root

app.Run();
