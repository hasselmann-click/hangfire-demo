
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;

namespace Hangfire.Runner.Filters;

public class SkippableDisableConcurrentExecutionAttribute : JobFilterAttribute, IServerFilter
{
    public const string SkipItemKey = "SkipConcurrentExecution";

    private readonly DisableConcurrentExecutionAttribute distributedLock;

    public SkippableDisableConcurrentExecutionAttribute(int timeoutInSeconds)
    {
        distributedLock = new DisableConcurrentExecutionAttribute(timeoutInSeconds);
    }

    public void OnPerforming(PerformingContext filterContext)
    {
        if (filterContext.Items.TryGetValue(SkipItemKey, out var skip) && (skip as bool?) == true)
        {
            return;
        }

        distributedLock.OnPerforming(filterContext);
    }

    public void OnPerformed(PerformedContext filterContext)
    {
        if (filterContext.Items.TryGetValue(SkipItemKey, out var skip) && (skip as bool?) == true)
        {
            return;
        }

        distributedLock.OnPerformed(filterContext);
    }

}