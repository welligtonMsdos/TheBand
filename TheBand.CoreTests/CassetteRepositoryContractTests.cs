using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;
using TheBand.CoreInfrastructure.Data;
using TheBand.CoreInfrastructure.Repositories;

namespace TheBand.CoreTests;

public sealed class CassetteRepositoryContractTests
{
    [Fact]
    public async Task ICassetteRepository_AddUpdateAndSoftDelete_UsesEfCore()
    {
        await using var context = new CoreContext(new DbContextOptionsBuilder<CoreContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        await using var dataSource = NpgsqlDataSource.Create("Host=localhost;Database=unused");
        ICassetteRepository repository = new CassetteRepository(context, dataSource);

        var cassette = new Cassette
        {
            Guid = Guid.NewGuid(),
            Artist = "Artist",
            Album = "Album",
            Year = 2020,
            Photo = "photo.jpg",
            Price = 10,
            Active = true,
            UserId = "user"
        };

        await repository.AddAsync(cassette, CancellationToken.None);

        cassette.Price = 20;

        Assert.True(await repository.UpdateAsync(cassette, "user", CancellationToken.None));
        Assert.Equal(20, context.Cassettes.Single().Price);
        Assert.True(await repository.DeleteAsync(cassette.Guid, "user", CancellationToken.None));
        Assert.False(context.Cassettes.Single().Active);
        Assert.False(await repository.DeleteAsync(Guid.NewGuid(), "user", CancellationToken.None));
    }
}
