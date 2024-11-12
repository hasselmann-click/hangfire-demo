using Common.Config;
using Common.Job;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace DynamicConcurrencyBatchJob;

public class DynamicConcurrencyBatchJob(AppsettingsDto appsettings, ILogger<DynamicConcurrencyBatchJob> logger, IBackgroundJobClient jobClient) : AbstractJob(Identifier, appsettings)
{
    private const string Identifier = "DynamicConcurrencyBatchJob";
    private const string Queue = "my-worker-queue";

    protected override Task ExecuteInnerAsync(CancellationToken stoppingToken)
    {
        var workersStr = Appsettings.Hangfire.Jobs[Identifier].Custom!["workers"];
        int workers = int.Parse(workersStr);

        logger.LogInformation("Start scheduling workers");
        for (int i = 1; i <= workers; ++i)
        {
            jobClient.Enqueue<DynamicConcurrencyBatchJobWorker>(Queue, w => w.ExecuteAsync(i, 3 * 1000, CancellationToken.None));
        }
        logger.LogInformation("Scheduled {workers} workers.", workers);
        return Task.CompletedTask;
    }
}
