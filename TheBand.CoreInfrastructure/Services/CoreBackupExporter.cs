using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreInfrastructure.Data;

namespace TheBand.CoreInfrastructure.Services;

public sealed class CoreBackupExporter(CoreContext context) : ICoreBackupExporter
{
    public async Task ExportAsync(TextWriter writer, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(writer);

        await writer.WriteLineAsync("[Cassette]".AsMemory(), cancellationToken);

        await writer.WriteLineAsync("Guid;Artist;Album;Year;Photo;Price;Active;UserId".AsMemory(), cancellationToken);

        await foreach (var item in context.Cassettes.AsNoTracking().OrderBy(item => item.Guid).AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            await WriteRowAsync(writer, cancellationToken, item.Guid, item.Artist, item.Album, item.Year, item.Photo, item.Price, item.Active, item.UserId);
        }

        await writer.WriteLineAsync(ReadOnlyMemory<char>.Empty, cancellationToken);

        await writer.WriteLineAsync("[CD]".AsMemory(), cancellationToken);

        await writer.WriteLineAsync("Guid;Artist;Album;Year;Photo;Price;Active;UserId".AsMemory(), cancellationToken);

        await foreach (var item in context.CDs.AsNoTracking().OrderBy(item => item.Guid).AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            await WriteRowAsync(writer, cancellationToken, item.Guid, item.Artist, item.Album, item.Year, item.Photo, item.Price, item.Active, item.UserId);
        }

        await writer.WriteLineAsync(ReadOnlyMemory<char>.Empty, cancellationToken);

        await writer.WriteLineAsync("[Concert]".AsMemory(), cancellationToken);

        await writer.WriteLineAsync("Guid;Artist;Venue;ShowDate;Photo;Price;Active;UserId".AsMemory(), cancellationToken);

        await foreach (var item in context.Concerts.AsNoTracking().OrderBy(item => item.Guid).AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            await WriteRowAsync(writer, cancellationToken, item.Guid, item.Artist, item.Venue, item.ShowDate, item.Photo, item.Price, item.Active, item.UserId);
        }

        await writer.WriteLineAsync(ReadOnlyMemory<char>.Empty, cancellationToken);

        await writer.WriteLineAsync("[Vinyl]".AsMemory(), cancellationToken);

        await writer.WriteLineAsync("Guid;Artist;Album;Year;Photo;Price;Active;UserId".AsMemory(), cancellationToken);

        await foreach (var item in context.Vinyls.AsNoTracking().OrderBy(item => item.Guid).AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            await WriteRowAsync(writer, cancellationToken, item.Guid, item.Artist, item.Album, item.Year, item.Photo, item.Price, item.Active, item.UserId);
        }
    }

    private static Task WriteRowAsync(TextWriter writer, CancellationToken cancellationToken, params object?[] values)
    {
        var fields = values.Select(value => Escape(value switch
        {
            null => string.Empty,
            DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        }));

        return writer.WriteLineAsync(string.Join(';', fields).AsMemory(), cancellationToken);
    }

    private static string Escape(string value) =>
        value.Contains(';') || value.Contains('"') || value.Contains('\r') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}
