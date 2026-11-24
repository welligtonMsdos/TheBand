namespace TheBand.CoreApplication.Interfaces;

public interface ICoreBackupExporter
{
    Task ExportAsync(TextWriter writer, CancellationToken cancellationToken);
}
