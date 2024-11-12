namespace Common.Config;

public class HangfireConfig
{
    public string? Queue { get; set; }

    public required Dictionary<string, JobConfig> Jobs { get; set; }
    public int WorkerCount { get; set; }
}

public class JobConfig
{
    public required string Cron { get; set; }
    public bool TurnOn { get; set; }
    public bool RunAtStartup { get; set; }
    public Dictionary<string, string>? Custom { get; set; }
}

