namespace Common.Job;

public interface IGenericJob
{

    string JobIdentifier { get; }
    
    Task ExecuteAsync(CancellationToken stoppingToken);
}

