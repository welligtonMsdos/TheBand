using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;
using TheBand.CoreInfrastructure.Data;
using TheBand.CoreInfrastructure.Repositories;

namespace TheBand.CoreTests;

public sealed class ConcertRepositoryContractTests
{
    [Fact]
    public async Task CommandRepository_AddUpdateAndSoftDelete_UsesEfCore()
    {
        await using var context = new CoreContext(
            new DbContextOptionsBuilder<CoreContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        await using var dataSource = NpgsqlDataSource.Create("Host=localhost;Database=unused");

        IConcertRepository repository = new ConcertRepository(context, dataSource);
        var concert = new Concert
        {
            Guid = Guid.NewGuid(),
            Artist = "Artist",
            Venue = "Venue",
            ShowDate = new DateOnly(2026, 10, 1),
            Photo = "photo.jpg",
            Active = true,
            UserId = "user"
        };

        await repository.AddAsync(concert, CancellationToken.None);

        Assert.Single(context.Concerts);

        concert.Venue = "New Venue";

        Assert.True(await repository.UpdateAsync(concert, "user", CancellationToken.None));
        Assert.Equal("New Venue", context.Concerts.Single().Venue);

        Assert.True(await repository.DeleteAsync(concert.Guid, "user", CancellationToken.None));
        Assert.False(context.Concerts.Single().Active);
        Assert.False(await repository.DeleteAsync(Guid.NewGuid(), "user", CancellationToken.None));
    }
}
