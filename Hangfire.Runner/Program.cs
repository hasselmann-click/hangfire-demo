// See https://aka.ms/new-console-template for more information
using Common.Hangfire;
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
                configApp.AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: false)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
            })
            .ConfigureLogging((hostContext, configLogging) =>
            {
                configLogging.AddConsole();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services
                    .AddHangfire(hostContext.Configuration)
                    .AddHostedService<HostedBackgroundService>();
            })
            .Build();

        await host.RunAsync();
    }
}