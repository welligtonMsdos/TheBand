using System.Globalization;
using System.Text;
using TheBand.CoreApplication.Interfaces;

namespace TheBand.CoreWorker;

public sealed class BackupWorker(
    IServiceScopeFactory scopeFactory,
    IHostEnvironment environment,
    ILogger<BackupWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

        do
        {
            try
            {
                await CreateBackupAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Falha ao exportar o backup do Core.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CreateBackupAsync(CancellationToken cancellationToken)
    {
        var directory = Path.Combine(environment.ContentRootPath, "bkp");

        Directory.CreateDirectory(directory);

        var fileName = DateTime.Now.ToString("ddMMyyyyHHmmss", CultureInfo.InvariantCulture) + ".txt";

        var destination = Path.Combine(directory, fileName);

        var temporary = Path.Combine(directory, $".{Guid.NewGuid():N}.tmp");

        try
        {
            await using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 65536, useAsync: true))
            await using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                await using var scope = scopeFactory.CreateAsyncScope();

                var exporter = scope.ServiceProvider.GetRequiredService<ICoreBackupExporter>();

                await exporter.ExportAsync(writer, cancellationToken);
            }

            File.Move(temporary, destination);

            logger.LogInformation("Backup criado em {Path}.", destination);
        }
        finally
        {
            if (File.Exists(temporary))
            {
                File.Delete(temporary);
            }
        }
    }
}
