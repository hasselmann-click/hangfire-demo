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
        using var host = new HostBuilder()
            .UseEnvironment(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Development)
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                configApp
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
            })
            .ConfigureLogging((hostContext, configLogging) =>
            {
                configLogging.AddConsole();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var appsettings = hostContext.Configuration.Get<AppsettingsDto>() ?? throw new Exception("Could not load appsettings");
                services
                    .AddSingleton(appsettings)
                    .AddCustomHangfireServer(appsettings, GlobalConfiguration.Configuration)
                    .AddHostedService<HostedBackgroundService>()

                    // add jobs here
                    .AddTransient<IGenericJob, HelloWorldJob.HelloWorldJob>()
                    .AddTransient<IGenericJob, DynamicConcurrencyBatchJob.DynamicConcurrencyBatchJob>()
                    ;
            })
            .Build();

        await host.RunAsync();
    }
}