using Common.Config;
using Common.Hangfire;
using Common.Job;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hangfire.Runner;

public class HostedBackgroundService(ILogger<HostedBackgroundService> logger, IEnumerable<IGenericJob> jobs,
    IConfiguration configuration, IRecurringJobManager jobManager, JobActivator jobActivator) : BackgroundService
{

    private readonly ILogger<HostedBackgroundService> logger = logger;
    private readonly List<IGenericJob> jobs = jobs.ToList();
    private readonly IRecurringJobManager jobManager = jobManager;
    private readonly JobActivator jobActivator = jobActivator;
    private readonly AppsettingsDto appsettings = configuration.Get<AppsettingsDto>() ?? throw new Exception("Could not load appsettings");

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await base.StartAsync(cancellationToken);

            // job configs
            var jobDict = jobs.ToDictionary(j => j.JobIdentifier, StringComparer.InvariantCultureIgnoreCase);
            var implementedJobIds = jobDict.Keys;
            var jobConfigs = appsettings.Hangfire.Jobs;
            var queue = appsettings.Hangfire.Queue;

            // fetch recurring job ids from database
            var recurringJobIds = new List<string>(jobConfigs.Count);
            using var connection = new SqlConnection(appsettings.Database.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            var query = $"SELECT [Key] FROM [Hangfire].[Hash] where Field='Queue' AND Value=@queue";
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@queue", queue);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var key = reader.GetString(0);
                recurringJobIds.Add(key);
            }

            // remove old or deactivated jobs
            var toRemove = recurringJobIds.Where(jid => !jobConfigs.ContainsKey(jid));
            if (toRemove.Any())
            {
                logger.LogWarning("Removing jobs with ids [{ids}]", string.Join(", ", toRemove));
                foreach (var id in toRemove)
                {
                    jobManager.RemoveIfExists(id);
                }
            }

            // warn about jobs without config
            var jobsMissingConfig = implementedJobIds.Except(jobConfigs.Keys, StringComparer.InvariantCultureIgnoreCase);
            if (jobsMissingConfig.Any())
            {
                logger.LogWarning("Jobs without config detected: [{jobs}]", string.Join(", ", jobsMissingConfig));
            }

            // schedule or update the configured jobs
            logger.LogInformation("Scheduling jobs [{keys)}]", string.Join(", ", jobConfigs.Keys));
            foreach (var hfj in jobConfigs)
            {
                try
                {
                    var job = jobDict[hfj.Key];
                    // hangfire uses the method expression to get all the metadata it needs to deserizalize and activate the job later on
                    jobManager.AddOrUpdate(hfj.Key, queue, () => job.ExecuteAsync(CancellationToken.None),
                        hfj.Value.Cron, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error scheduling job [{jkey}]. Inner exception: [{ie}: {iem}]",
                        hfj.Key, ex.InnerException?.GetType(), ex.InnerException?.Message);
                    continue;
                }

                // trigger jobs immediately if configured no matter their schedule
                if (hfj.Value.RunAtStartup == true)
                {
                    jobManager.Trigger(hfj.Key);
                }
#if DEBUG // run anyway
                else
                {
                    // in debug we trigger every job anyway and use the turn on flag to decide if it should actually be executed
                    logger.LogWarning($"Debug directive: Triggering {hfj.Key} anyway.");
                    jobManager.Trigger(hfj.Key);
                }
#endif
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting background service");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // actually running the hangfire server
        var hangfireConfig = appsettings.Hangfire;
        using var server = new BackgroundJobServer(new BackgroundJobServerOptions
        {
            WorkerCount = hangfireConfig.WorkerCount,
            Queues = [hangfireConfig.Queue],
            Activator = jobActivator
        });

        await server.WaitForShutdownAsync(stoppingToken);
    }
}
