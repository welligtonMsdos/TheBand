using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreApplication.Models;
using TheBand.CoreApplication.Services;

namespace TheBand.CoreTests;

public sealed class CollectionPathServiceTests
{
    [Fact]
    public async Task GetActivePathsAsync_MapsPathsAndForwardsUserAndCancellation()
    {
        var repository = new FakeRepository
        {
            Paths = [new CollectionPathItem("cassette.jpg", "cassette"), new CollectionPathItem("cd.jpg", "cd")]
        };

        ICollectionPathService service = new CollectionPathService(repository);

        using var cancellation = new CancellationTokenSource();

        var paths = await service.GetActivePathsAsync("user-1", cancellation.Token);

        Assert.Equal(
            [new CollectionPathDto("cassette.jpg", "cassette"), new CollectionPathDto("cd.jpg", "cd")],
            paths);

        Assert.Equal("user-1", repository.UserId);

        Assert.Equal(cancellation.Token, repository.CancellationToken);
    }

    [Fact]
    public async Task GetActivePathsAsync_EmptyRepositoryResult_ReturnsEmptyCollection()
    {
        ICollectionPathService service = new CollectionPathService(new FakeRepository());

        Assert.Empty(await service.GetActivePathsAsync("user-1", CancellationToken.None));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetActivePathsAsync_InvalidUserId_DoesNotQueryRepository(string? userId)
    {
        var repository = new FakeRepository();

        ICollectionPathService service = new CollectionPathService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetActivePathsAsync(userId!, CancellationToken.None));

        Assert.Null(repository.UserId);
    }

    [Fact]
    public async Task GetActivePathsAsync_RepositoryFailure_PropagatesFailure()
    {
        var failure = new InvalidOperationException("Database unavailable.");

        ICollectionPathService service = new CollectionPathService(new FakeRepository { Failure = failure });

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetActivePathsAsync("user-1", CancellationToken.None));

        Assert.Same(failure, exception);
    }

    private sealed class FakeRepository : ICollectionPathRepository
    {
        public IReadOnlyCollection<CollectionPathItem> Paths { get; set; } = [];

        public Exception? Failure { get; set; }

        public string? UserId { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public Task<IReadOnlyCollection<CollectionPathItem>> GetActivePathsAsync(string userId, CancellationToken cancellationToken)
        {
            UserId = userId;

            CancellationToken = cancellationToken;

            return Failure is null ? Task.FromResult(Paths) : Task.FromException<IReadOnlyCollection<CollectionPathItem>>(Failure);
        }
    }
}
