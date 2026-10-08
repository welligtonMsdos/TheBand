using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreApplication.Services;

namespace TheBand.CoreTests;

public sealed class VinylPhotoServiceTests
{
    [Theory]

    [InlineData(10, 1, 1, false)]

    [InlineData(11, 1, 2, true)]

    [InlineData(20, 1, 2, true)]

    [InlineData(20, 2, 2, false)]

    [InlineData(21, 2, 3, true)]

    public async Task GetPhotosAsync_MetadataIdentifiesNextPageIncludingFullLastPage(int totalItems, int page, int totalPages, bool hasNextPage)
    {
        var repository = new FakeVinylRepository();

        IVinylService service = new VinylService(repository);

        for (var index = 0; index < totalItems; index++)
        {
            await service.CreateAsync("user-1", new CreateVinylDto("Artist", "Album", 2000 + index, $"photo-{index}.jpg", 10), CancellationToken.None);
        }

        var result = await service.GetPhotosAsync("user-1", page, CancellationToken.None);

        Assert.Equal(totalItems, result.TotalItems);

        Assert.Equal(totalPages, result.TotalPages);

        Assert.Equal(hasNextPage, result.HasNextPage);

        Assert.Equal(10, result.Items.Count);
    }

    [Fact]

    public async Task GetPhotosAsync_CountFailure_PropagatesFailure()
    {
        var failure = new InvalidOperationException("Count unavailable.");

        var repository = new FakeVinylRepository { CountException = failure };

        IVinylService service = new VinylService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetPhotosAsync("user-1", 1, CancellationToken.None));

        Assert.Same(failure, exception);
    }

    [Theory]

    [InlineData(1, 10)]

    [InlineData(2, 10)]

    [InlineData(3, 3)]

    [InlineData(4, 0)]

    [InlineData(int.MaxValue, 0)]

    public async Task GetPhotosAsync_ReturnsRequestedPageWithAtMostTenItems(int page, int expectedCount)
    {
        var repository = new FakeVinylRepository();

        IVinylService service = new VinylService(repository);

        var expected = new List<VinylPhotoDto>();

        for (var index = 0; index < 23; index++)
        {
            var vinyl = await service.CreateAsync("user-1", new CreateVinylDto("Artist", "Album", 2000 + index, $"photo-{index}.jpg", 10), CancellationToken.None);

            expected.Add(new VinylPhotoDto(vinyl.Guid, vinyl.Photo));
        }

        await service.CreateAsync("user-2", new CreateVinylDto("Artist", "Album", 1999, "other.jpg", 10), CancellationToken.None);

        var deleted = await service.CreateAsync("user-1", new CreateVinylDto("Artist", "Album", 1998, "deleted.jpg", 10), CancellationToken.None);

        await service.DeleteAsync(deleted.Guid, "user-1", CancellationToken.None);

        var result = await service.GetPhotosAsync("user-1", page, CancellationToken.None);

        Assert.Equal(expectedCount, result.Items.Count);

        Assert.Equal(page, result.Page);

        Assert.Equal(10, result.PageSize);

        Assert.Equal(23, result.TotalItems);

        Assert.Equal(3, result.TotalPages);

        Assert.Equal(page < 3, result.HasNextPage);

        var expectedPage = expected.Where((_, index) => index / 10 == page - 1);

        Assert.Equal(expectedPage, result.Items);

        Assert.Equal(((long)page - 1) * 10, repository.LastPhotoOffset);

        Assert.Equal(10, repository.LastPhotoPageSize);
    }

    [Theory]

    [InlineData(0)]

    [InlineData(-1)]

    public async Task GetPhotosAsync_InvalidPage_RejectsRequestBeforeCallingRepository(int page)
    {
        var repository = new FakeVinylRepository();

        IVinylService service = new VinylService(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetPhotosAsync("user-1", page, CancellationToken.None));

        Assert.Null(repository.LastPhotoUserId);

        Assert.Null(repository.LastCountUserId);
    }

    [Fact]

    public async Task GetPhotosAsync_CanceledRequest_PropagatesCancellation()
    {
        IVinylService service = new VinylService(new FakeVinylRepository());

        using var cancellation = new CancellationTokenSource();

        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.GetPhotosAsync("user-1", 1, cancellation.Token));
    }

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

        var result = await service.GetPhotosAsync("user-1", 1, CancellationToken.None);

        Assert.Equal([new VinylPhotoDto(first.Guid, first.Photo), new VinylPhotoDto(second.Guid, second.Photo)], result.Items);

        Assert.Equal(2, result.TotalItems);

        Assert.Equal(1, result.TotalPages);

        Assert.False(result.HasNextPage);
    }

    [Fact]

    public async Task GetPhotosAsync_NoVinyls_ReturnsEmptyCollection()
    {
        IVinylService service = new VinylService(new FakeVinylRepository());

        var result = await service.GetPhotosAsync("user-1", 1, CancellationToken.None);

        Assert.Empty(result.Items);

        Assert.Equal(1, result.Page);

        Assert.Equal(10, result.PageSize);

        Assert.Equal(0, result.TotalItems);

        Assert.Equal(0, result.TotalPages);

        Assert.False(result.HasNextPage);
    }

    [Theory]

    [InlineData(null)]

    [InlineData("")]

    [InlineData(" ")]

    public async Task GetPhotosAsync_InvalidUserId_RejectsRequestBeforeCallingRepository(string? userId)
    {
        var repository = new FakeVinylRepository();

        IVinylService service = new VinylService(repository);

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetPhotosAsync(userId!, 1, CancellationToken.None));

        Assert.Null(repository.LastPhotoUserId);

        Assert.Null(repository.LastCountUserId);
    }

    [Fact]

    public async Task GetPhotosAsync_ForwardsUserIdAndCancellationToken()
    {
        var repository = new FakeVinylRepository();

        IVinylService service = new VinylService(repository);

        using var cancellation = new CancellationTokenSource();

        await service.GetPhotosAsync("user-1", 2, cancellation.Token);

        Assert.Equal("user-1", repository.LastPhotoUserId);

        Assert.Equal(cancellation.Token, repository.LastPhotoCancellationToken);

        Assert.Equal("user-1", repository.LastCountUserId);

        Assert.Equal(cancellation.Token, repository.LastCountCancellationToken);

        Assert.Equal(10, repository.LastPhotoOffset);

        Assert.Equal(10, repository.LastPhotoPageSize);
    }

    [Fact]

    public async Task GetPhotosAsync_RepositoryFailure_PropagatesFailure()
    {
        var failure = new InvalidOperationException("Persistence unavailable.");

        var repository = new FakeVinylRepository { GetAllException = failure };

        IVinylService service = new VinylService(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetPhotosAsync("user-1", 1, CancellationToken.None));

        Assert.Same(failure, exception);
    }
}
