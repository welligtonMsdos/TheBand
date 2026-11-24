using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreDomain.Entities;
using TheBand.CoreInfrastructure.Data;
using TheBand.CoreInfrastructure.Services;

namespace TheBand.CoreTests;

public sealed class CoreBackupExporterContractTests
{
    [Fact]
    public async Task ExportAsync_WritesAllTablesAndFieldsWithInvariantValues()
    {
        await using var context = CreateContext();

        var cassetteId = Guid.NewGuid();

        context.Cassettes.Add(new Cassette { Guid = cassetteId, Artist = "A;B", Album = "Line\nTwo", Year = 1980, Photo = "c.jpg", Price = 12.50m, Active = false, UserId = "user-1" });

        context.CDs.Add(new CD { Guid = Guid.NewGuid(), Artist = "C", Album = "D", Year = 1990, Photo = "cd.jpg", Price = 1.25m, Active = true, UserId = "user-2" });

        context.Concerts.Add(new Concert { Guid = Guid.NewGuid(), Artist = "E", Venue = "The \"Room\"", ShowDate = new DateOnly(2020, 1, 2), Photo = "show.jpg", Price = 2.50m, Active = true, UserId = "user-3" });

        context.Vinyls.Add(new Vinyl { Guid = Guid.NewGuid(), Artist = "F", Album = "G", Year = 2000, Photo = "v.jpg", Price = 3.75m, Active = true, UserId = "user-4" });

        await context.SaveChangesAsync();

        ICoreBackupExporter exporter = new CoreBackupExporter(context);

        using var writer = new StringWriter(CultureInfo.InvariantCulture);

        await exporter.ExportAsync(writer, CancellationToken.None);

        var output = writer.ToString();

        Assert.Contains("[Cassette]\nGuid;Artist;Album;Year;Photo;Price;Active;UserId", Normalize(output));

        Assert.Contains($"{cassetteId};\"A;B\";\"Line\nTwo\";1980;c.jpg;12.50;False;user-1", Normalize(output));

        Assert.Contains("[CD]\nGuid;Artist;Album;Year;Photo;Price;Active;UserId", Normalize(output));

        Assert.Contains("[Concert]\nGuid;Artist;Venue;ShowDate;Photo;Price;Active;UserId", Normalize(output));

        Assert.Contains(";\"The \"\"Room\"\"\";2020-01-02;show.jpg;2.50;True;user-3", Normalize(output));

        Assert.Contains("[Vinyl]\nGuid;Artist;Album;Year;Photo;Price;Active;UserId", Normalize(output));
    }

    [Fact]
    public async Task ExportAsync_WithoutRecords_WritesAllSectionHeaders()
    {
        await using var context = CreateContext();

        ICoreBackupExporter exporter = new CoreBackupExporter(context);

        using var writer = new StringWriter();

        await exporter.ExportAsync(writer, CancellationToken.None);

        Assert.Equal(4, writer.ToString().Split("Guid;Artist;").Length - 1);
    }

    [Fact]
    public async Task ExportAsync_CanceledRequest_PropagatesCancellation()
    {
        await using var context = CreateContext();

        ICoreBackupExporter exporter = new CoreBackupExporter(context);

        using var writer = new StringWriter();

        using var cancellation = new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => exporter.ExportAsync(writer, cancellation.Token));
    }

    private static CoreContext CreateContext() =>
        new(new DbContextOptionsBuilder<CoreContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static string Normalize(string value) => value.Replace("\r\n", "\n");
}
