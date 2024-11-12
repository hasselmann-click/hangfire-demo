using Common.Config;
using Common.Hangfire.Filters;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Hangfire;
public static class ServiceRegistry
{
    public static IServiceCollection AddCustomHangfireServer(this IServiceCollection services, AppsettingsDto appsettings, IGlobalConfiguration globalConfiguration)
    {
        var serviceProvider = services.BuildServiceProvider();
        AddCustomHangfire(appsettings, globalConfiguration, serviceProvider);

        return services
            .AddTransient<JobActivator, ContainerJobActivator>()
            .AddScoped<IRecurringJobManager, RecurringJobManager>()
            ;
    }

    public static void AddCustomHangfire(AppsettingsDto appsettings, IGlobalConfiguration globalConfiguration, IServiceProvider serviceProvider)
    {
        var dbConfig = appsettings.Database;
        globalConfiguration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseColouredConsoleLogProvider()
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(dbConfig.ConnectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            })
            .UseActivator(new ContainerJobActivator(serviceProvider))
            .UseFilter(new AutomaticRetryAttribute { Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Delete })
            .UseFilter(new PreserveOriginalQueueAttribute())
            .UseFilter(new SkippableDisableConcurrentExecutionAttribute(timeoutInSeconds: 5))
            ;
    }
}
