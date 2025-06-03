namespace Aspire.AppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        // setup storage
        IResourceBuilder<IResourceWithConnectionString> connectionStringResource;
#if DEBUG
        // Use a container for SQL Server in development
        var sqlServer = builder.AddSqlServer("sql-server");
        connectionStringResource = sqlServer.AddDatabase("ApplicationDB");
#else
        // Define a secret parameter for the SQL Server connection string
        var connectionStringParam = builder.AddParameter("sql-server-connection-string", secret: true);
        // Use a connection string from a secret parameter for SQL Server in production
        connectionStringResource = builder
            .AddConnectionString("sql-server-connection-string", ReferenceExpression.Create($"{connectionStringParam}"));
#endif

        builder.AddProject<Projects.Hangfire_Runner>("hangfire-runner")
            .WithReference(connectionStringResource)
            .WaitFor(connectionStringResource)
            // Aspire uses "ConnectionStrings__ConnectionName" by default as name
            // We remap this here explicitly thus touching the existing config as little as possible
            .WithEnvironment("Database__ConnectionString", connectionStringResource);

        builder.AddProject<Projects.Hangfire_Dashboard>("hangfire-dashboard")
            .WithReference(connectionStringResource)
            .WaitFor(connectionStringResource)
            .WithEnvironment("Database__ConnectionString", connectionStringResource)
            .WithExternalHttpEndpoints();

        builder.Build().Run();
    }
}