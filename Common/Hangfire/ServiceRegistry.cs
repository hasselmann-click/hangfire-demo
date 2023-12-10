using Common.Config;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Hangfire;
public static class ServiceRegistry
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration config)
    {
        var dbConfig = config.Get<AppsettingsDto>()?.Database ?? throw new Exception("Database configuration not found");
        GlobalConfiguration.Configuration
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
            });

        return services;
    }
}
