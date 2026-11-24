using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreApplication.Services;

namespace TheBand.CoreTests;

public sealed class CollectionPathServiceTests
{
    [Fact]
    public async Task GetActivePathsAsync_MapsPathsAndForwardsCancellation()
    {
        var repository = new FakeRepository
        {
            Paths = [new CollectionPathDto("cassette.jpg", "cassette"), new CollectionPathDto("cd.jpg", "cd")]
        };

        ICollectionPathService service = new CollectionPathService(repository);

        using var cancellation = new CancellationTokenSource();

        var paths = await service.GetActivePathsAsync(cancellation.Token);

        Assert.Equal([new CollectionPathDto("cassette.jpg", "cassette"), new CollectionPathDto("cd.jpg", "cd")], paths);

        Assert.Equal(cancellation.Token, repository.CancellationToken);
    }

    [Fact]
    public async Task GetActivePathsAsync_EmptyRepositoryResult_ReturnsEmptyCollection()
    {
        ICollectionPathService service = new CollectionPathService(new FakeRepository());

        Assert.Empty(await service.GetActivePathsAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetActivePathsAsync_RepositoryFailure_PropagatesFailure()
    {
        var failure = new InvalidOperationException("Database unavailable.");

        ICollectionPathService service = new CollectionPathService(new FakeRepository { Failure = failure });

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetActivePathsAsync(CancellationToken.None));

        Assert.Same(failure, exception);
    }

    private sealed class FakeRepository : ICollectionPathRepository
    {
        public IReadOnlyCollection<CollectionPathDto> Paths { get; set; } = [];

        public Exception? Failure { get; set; }

        public CancellationToken CancellationToken { get; private set; }

        public Task<IReadOnlyCollection<CollectionPathDto>> GetActivePathsAsync(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;

            return Failure is null ? Task.FromResult(Paths) : Task.FromException<IReadOnlyCollection<CollectionPathDto>>(Failure);
        }
    }
}
