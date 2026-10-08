using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreApplication.Services;

namespace TheBand.CoreTests;

public sealed class VinylPhotoServiceTests
{
    [Fact]

    public async Task GetPhotosAsync_ReturnsGuidAndPhotoOfActiveVinylsOwnedByUser()
    {
        var repository = new FakeVinylRepository();

        IVinylService service = new VinylService(repository);

        var first = await service.CreateAsync("user-1", new CreateVinylDto("Artist", "Album", 2020, "first.jpg", 10), CancellationToken.None);

        var second = await service.CreateAsync("user-1", new CreateVinylDto("Artist", "Album", 2021, "second.jpg", 20), CancellationToken.None);

        var deleted = await service.CreateAsync("user-1", new CreateVinylDto("Artist", "Album", 2022, "deleted.jpg", 30), CancellationToken.None);

        await service.CreateAsync("user-2", new CreateVinylDto("Artist", "Album", 2020, "other.jpg", 10), CancellationToken.None);

        await service.DeleteAsync(deleted.Guid, "user-1", CancellationToken.None);

        var result = await service.GetPhotosAsync("user-1", CancellationToken.None);

        Assert.Equal([new VinylPhotoDto(first.Guid, first.Photo), new VinylPhotoDto(second.Guid, second.Photo)], result);
    }

    [Fact]

    public async Task GetPhotosAsync_NoVinyls_ReturnsEmptyCollection()
    {
        IVinylService service = new VinylService(new FakeVinylRepository());

        Assert.Empty(await service.GetPhotosAsync("user-1", CancellationToken.None));
    }

    [Theory]

    [InlineData(null)]

    [InlineData("")]

    [InlineData(" ")]

    public async Task GetPhotosAsync_InvalidUserId_RejectsRequestBeforeCallingRepository(string? userId)
    {
        var repository = new FakeVinylRepository();

        IVinylService service = new VinylService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetPhotosAsync(userId!, CancellationToken.None));

        Assert.Null(repository.LastGetAllUserId);
    }

    [Fact]

    public async Task GetPhotosAsync_ForwardsUserIdAndCancellationToken()
    {
        var repository = new FakeVinylRepository();

        IVinylService service = new VinylService(repository);

        using var cancellation = new CancellationTokenSource();

        await service.GetPhotosAsync("user-1", cancellation.Token);

        Assert.Equal("user-1", repository.LastGetAllUserId);

        Assert.Equal(cancellation.Token, repository.LastGetAllCancellationToken);
    }

    [Fact]

    public async Task GetPhotosAsync_RepositoryFailure_PropagatesFailure()
    {
        var failure = new InvalidOperationException("Persistence unavailable.");

        var repository = new FakeVinylRepository { GetAllException = failure };

        IVinylService service = new VinylService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetPhotosAsync("user-1", CancellationToken.None));

        Assert.Same(failure, exception);
    }
}
