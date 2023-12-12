
using Common.Config;

namespace Common.Job;

public abstract class AbstractJob(string identifier, AppsettingsDto appsettings) : IGenericJob
{
    public string JobIdentifier { get; } = identifier;
    protected AppsettingsDto Appsettings { get; } = appsettings;

    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // check if job is turned on
        if(!Appsettings.Hangfire.Jobs[JobIdentifier].TurnOn)
        {
            return Task.CompletedTask;
        }

        return ExecuteInnerAsync(stoppingToken);
    }

    protected abstract Task ExecuteInnerAsync(CancellationToken stoppingToken);
}