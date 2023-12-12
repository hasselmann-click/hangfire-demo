namespace Common.Config;

public class AppsettingsDto
{
    public required HangfireConfig Hangfire { get; set; }
    public required DatabaseConfig Database { get; set; }

    public class DatabaseConfig
    {
        public required string ConnectionString { get; set; }
    }
}

