using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hangfire.Runner
{
    public class HostedBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));

            using var server = new BackgroundJobServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1
            });

            await server.WaitForShutdownAsync(stoppingToken);
        }
    }
}


