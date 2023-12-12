using Hangfire.Common;
using Hangfire.States;

namespace Hangfire.Runner.Filters;

public class PreserveOriginalQueueAttribute : JobFilterAttribute, IElectStateFilter
{
    private const string Param_OriginalQueue = "OriginalQueue";

    public void OnStateElection(ElectStateContext context)
    {
        if (context.CandidateState is not EnqueuedState enqueuedState)
        {
            return;
        }

        var originalQueue = context.GetJobParameter<string>(Param_OriginalQueue);
        if (string.IsNullOrEmpty(originalQueue))
        {
            context.Connection.SetJobParameter(context.BackgroundJob.Id, Param_OriginalQueue,
                SerializationHelper.Serialize(enqueuedState.Queue));
        }

        enqueuedState.Queue = originalQueue;
    }

}