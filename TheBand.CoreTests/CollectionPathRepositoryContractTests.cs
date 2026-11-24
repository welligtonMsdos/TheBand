using Microsoft.EntityFrameworkCore;
using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreDomain.Entities;
using TheBand.CoreInfrastructure.Data;
using TheBand.CoreInfrastructure.Repositories;

namespace TheBand.CoreTests;

public sealed class CollectionPathRepositoryContractTests
{
    [Fact]
    public async Task GetActivePathsAsync_ReturnsOnlyActivePhotosForRequestedUserFromAllFormats()
    {
        await using var context = CreateContext();

        context.Cassettes.Add(new Cassette { Guid = Guid.NewGuid(), Artist = "A", Album = "A", Photo = "cassette.jpg", UserId = "user-1", Active = true });

        context.CDs.Add(new CD { Guid = Guid.NewGuid(), Artist = "A", Album = "A", Photo = "cd.jpg", UserId = "user-1", Active = true });

        context.Vinyls.Add(new Vinyl { Guid = Guid.NewGuid(), Artist = "A", Album = "A", Photo = "vinyl.jpg", UserId = "user-1", Active = true });

        context.Cassettes.Add(new Cassette { Guid = Guid.NewGuid(), Artist = "A", Album = "A", Photo = "inactive.jpg", UserId = "user-1", Active = false });

        context.CDs.Add(new CD { Guid = Guid.NewGuid(), Artist = "A", Album = "A", Photo = "other-user.jpg", UserId = "user-2", Active = true });

        await context.SaveChangesAsync();

        ICollectionPathRepository repository = new CollectionPathRepository(context);

        var paths = await repository.GetActivePathsAsync("user-1", CancellationToken.None);

        Assert.Equal(
            [
                new CollectionPathDto("cassette.jpg", "cassette"),
                new CollectionPathDto("cd.jpg", "cd"),
                new CollectionPathDto("vinyl.jpg", "vinyl")
            ],
            paths);
    }

    [Fact]
    public async Task GetActivePathsAsync_WithoutMatchingRecords_ReturnsEmptyCollection()
    {
        await using var context = CreateContext();

        ICollectionPathRepository repository = new CollectionPathRepository(context);

        Assert.Empty(await repository.GetActivePathsAsync("user-1", CancellationToken.None));
    }

    [Fact]
    public async Task GetActivePathsAsync_CanceledRequest_PropagatesCancellation()
    {
        await using var context = CreateContext();

        ICollectionPathRepository repository = new CollectionPathRepository(context);

        using var cancellation = new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => repository.GetActivePathsAsync("user-1", cancellation.Token));
    }

    private static CoreContext CreateContext() =>
        new(new DbContextOptionsBuilder<CoreContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
