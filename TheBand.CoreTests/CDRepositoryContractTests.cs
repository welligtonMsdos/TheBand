using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;
using TheBand.CoreInfrastructure.Data;
using TheBand.CoreInfrastructure.Repositories;

namespace TheBand.CoreTests;

public sealed class CDRepositoryContractTests
{
    [Fact]
    public async Task ICDRepository_AddUpdateAndSoftDelete_UsesEfCore()
    {
        await using var context = new CoreContext(new DbContextOptionsBuilder<CoreContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        await using var dataSource = NpgsqlDataSource.Create("Host=localhost;Database=unused");
        ICDRepository repository = new CDRepository(context, dataSource);

        var cd = new CD
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

        await repository.AddAsync(cd, CancellationToken.None);

        cd.Price = 20;

        Assert.True(await repository.UpdateAsync(cd, "user", CancellationToken.None));
        Assert.Equal(20, context.CDs.Single().Price);
        Assert.True(await repository.DeleteAsync(cd.Guid, "user", CancellationToken.None));
        Assert.False(context.CDs.Single().Active);
        Assert.False(await repository.DeleteAsync(Guid.NewGuid(), "user", CancellationToken.None));
    }
}
