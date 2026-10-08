using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;
using TheBand.CoreInfrastructure.Data;
using TheBand.CoreInfrastructure.Repositories;

namespace TheBand.CoreTests;

public sealed class VinylRepositoryContractTests
{
    [Fact]

    public async Task CountActiveAsync_CanceledRequest_PropagatesCancellation()
    {
        await using var context = new CoreContext(new DbContextOptionsBuilder<CoreContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        await using var dataSource = NpgsqlDataSource.Create("Host=localhost;Database=unused");

        IVinylRepository repository = new VinylRepository(context, dataSource);

        using var cancellation = new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => repository.CountActiveAsync("user-1", cancellation.Token));
    }

    [Fact]

    public async Task GetPhotosAsync_CanceledRequest_PropagatesCancellation()
    {
        await using var context = new CoreContext(new DbContextOptionsBuilder<CoreContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        await using var dataSource = NpgsqlDataSource.Create("Host=localhost;Database=unused");

        IVinylRepository repository = new VinylRepository(context, dataSource);

        using var cancellation = new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => repository.GetPhotosAsync("user-1", 0, 10, cancellation.Token));
    }

    [Fact]
    public async Task CommandRepository_AddUpdateAndSoftDelete_UsesEfCore()
    {
        await using var context = new CoreContext(new DbContextOptionsBuilder<CoreContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        await using var dataSource = NpgsqlDataSource.Create("Host=localhost;Database=unused");

        IVinylRepository repository = new VinylRepository(context, dataSource);

        var vinyl = new Vinyl { Guid = Guid.NewGuid(), Artist = "Artist", Album = "Album", Year = 2020, Photo = "photo.jpg", Price = 10, Active = true, UserId = "user" };
       
        await repository.AddAsync(vinyl, CancellationToken.None);

        Assert.Single(context.Vinyls);

        vinyl.Price = 20;

        Assert.True(await repository.UpdateAsync(vinyl, "user", CancellationToken.None));

        Assert.True(await repository.DeleteAsync(vinyl.Guid, "user", CancellationToken.None));

        Assert.False(context.Vinyls.Single().Active);

        Assert.False(await repository.DeleteAsync(Guid.NewGuid(), "user", CancellationToken.None));
    }
}
