using Hangfire;
using Common.Hangfire;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHangfire((sp, config)  => {
    ServiceRegistry.ConfigureHangfireClient(builder.Configuration, config);
});
var app = builder.Build();

app.UseHangfireDashboard(""); // map to root

app.Run();
