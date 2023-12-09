namespace Common.Config;

public class Configuration
{
    public required DatabaseConfig Database { get; set; }

    public class DatabaseConfig
    {
        public required string ConnectionString { get; set; }
    }
}

