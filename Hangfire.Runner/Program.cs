// See https://aka.ms/new-console-template for more information
using Common.Config;
using Common.Hangfire;
using Common.Job;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hangfire.Runner;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Configure host environment
        builder.Environment.EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Development;

        // Configure app configuration
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        // Configure logging
        builder.Logging.AddConsole();

        // Add service defaults first
        builder.AddServiceDefaults();

        // Configure services
        var appsettings = builder.Configuration.Get<AppsettingsDto>() ?? throw new Exception("Could not load appsettings");
        builder.Services
            .AddSingleton(appsettings)
            .AddCustomHangfireServer(appsettings, GlobalConfiguration.Configuration)
            .AddHostedService<HostedBackgroundService>()
            // add jobs here
            .AddTransient<IGenericJob, HelloWorldJob.HelloWorldJob>()
            .AddTransient<IGenericJob, DynamicConcurrencyBatchJob.DynamicConcurrencyBatchJob>();

        var host = builder.Build();

        await host.RunAsync();
    }
}