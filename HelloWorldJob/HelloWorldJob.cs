using Common.Config;
using Common.Job;
using Microsoft.Extensions.Logging;

namespace HelloWorldJob;

public class HelloWorldJob(AppsettingsDto appsettings, ILogger<HelloWorldJob> logger) : AbstractJob(Identifier, appsettings)
{
    private const string Identifier = "HelloWorldJob";
    private readonly ILogger<HelloWorldJob> logger = logger;

    protected override Task ExecuteInnerAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Hello World 2!");
        return Task.CompletedTask;
    }
}
