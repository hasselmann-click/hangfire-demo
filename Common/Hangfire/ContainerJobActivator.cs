using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Hangfire;

public class ContainerJobActivator(IServiceProvider serviceProvider) : JobActivator
{
    public override object ActivateJob(Type jobType)
    {
        return ActivatorUtilities.CreateInstance(serviceProvider, jobType);
    }
}