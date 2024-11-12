using Common.Hangfire.Filters;
using Microsoft.Extensions.Logging;

namespace DynamicConcurrencyBatchJob;

public class DynamicConcurrencyBatchJobWorker(ILogger<DynamicConcurrencyBatchJobWorker> logger) 
{
    [SkipConcurrentExecutionFilter]
    public async Task ExecuteAsync(int nonReferenceParam, int sleepMs, CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker running [{param}|{threadId}|{time}]", nonReferenceParam, Environment.CurrentManagedThreadId, DateTimeOffset.Now);
        await Task.Delay(sleepMs, stoppingToken);
        logger.LogInformation("Worker finished [{param}|{threadId}|{time}]", nonReferenceParam, Environment.CurrentManagedThreadId, DateTimeOffset.Now);
    }
    
}
