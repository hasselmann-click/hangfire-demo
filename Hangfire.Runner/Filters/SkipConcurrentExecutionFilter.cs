using Hangfire.Common;
using Hangfire.Server;

namespace Hangfire.Runner.Filters;

public class SkipConcurrentExecutionFilter : JobFilterAttribute, IServerFilter
{
    public SkipConcurrentExecutionFilter()
    {
        Order = 100;
    }

    public void OnPerforming(PerformingContext filterContext)
    {
        filterContext.Items[SkippableDisableConcurrentExecutionAttribute.SkipItemKey] = true;
    }

    public void OnPerformed(PerformedContext filterContext)
    {
        // do nothing
    }
}