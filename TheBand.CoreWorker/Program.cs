using TheBand.CoreInfrastructure.Data;
using TheBand.CoreWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCoreInfrastructure(builder.Configuration);

builder.Services.AddHostedService<BackupWorker>();

await builder.Build().RunAsync();
