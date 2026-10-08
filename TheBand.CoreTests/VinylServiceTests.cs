using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Services;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;

namespace TheBand.CoreTests;

public sealed class VinylServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsAndReturnsActiveVinyl()
    {
        var repository = new FakeVinylRepository();

        var service = new VinylService(repository);

        var result = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Guid);

        Assert.True(true);

        Assert.Single(repository.Items);
    }

    [Fact]
    public async Task GetByGuidAsync_MissingVinyl_ReturnsNull()
    {
        var service = new VinylService(new FakeVinylRepository());

        Assert.Null(await service.GetByGuidAsync(Guid.NewGuid(), "user-1", CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_MissingVinyl_ReturnsNull()
    {
        var service = new VinylService(new FakeVinylRepository());

        Assert.Null(await service.UpdateAsync(Guid.NewGuid(), "user-1", UpdateRequest(), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ExistingVinyl_UpdatesOnlyTheOwnersVinyl()
    {
        var repository = new FakeVinylRepository();
        var service = new VinylService(repository);
        var created = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        var updated = await service.UpdateAsync(created.Guid, "user-1", UpdateRequest(), CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(2021, updated.Year);
        Assert.Equal(150, updated.Price);
    }

    [Fact]
    public async Task DeleteAsync_ExistingVinyl_ReturnsTrueAndHidesIt()
    {
        var repository = new FakeVinylRepository();

        var service = new VinylService(repository);

        var created = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        Assert.True(await service.DeleteAsync(created.Guid, "user-1", CancellationToken.None));

        Assert.Null(await service.GetByGuidAsync(created.Guid, "user-1", CancellationToken.None));
    }

    [Fact]
    public async Task GetAllAsync_DifferentUser_DoesNotReturnAnotherUsersVinyl()
    {
        var repository = new FakeVinylRepository();

        var service = new VinylService(repository);

        await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        var result = await service.GetAllAsync("user-2", CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMostExpensiveAsync_ReturnsAtMostThreeOfTheUsersVinylsInDescendingPriceOrder()
    {
        var repository = new FakeVinylRepository();
        var service = new VinylService(repository);

        await service.CreateAsync("user-1", CreateRequest(price: 30), CancellationToken.None);
        await service.CreateAsync("user-1", CreateRequest(price: 100), CancellationToken.None);
        await service.CreateAsync("user-1", CreateRequest(price: 50), CancellationToken.None);
        await service.CreateAsync("user-1", CreateRequest(price: 75), CancellationToken.None);
        await service.CreateAsync("user-2", CreateRequest(price: 200), CancellationToken.None);

        var result = await service.GetMostExpensiveAsync("user-1", CancellationToken.None);

        Assert.Equal([100m, 75m, 50m], result.Select(vinyl => vinyl.Price));
    }

    [Fact]
    public async Task GetThreeCheapestAsync_ReturnsAtMostThreeOfTheUsersVinylsInAscendingPriceOrder()
    {
        var repository = new FakeVinylRepository();
        var service = new VinylService(repository);

        await service.CreateAsync("user-1", CreateRequest(price: 50), CancellationToken.None);
        await service.CreateAsync("user-1", CreateRequest(price: 10), CancellationToken.None);
        await service.CreateAsync("user-1", CreateRequest(price: 30), CancellationToken.None);
        await service.CreateAsync("user-1", CreateRequest(price: 20), CancellationToken.None);
        await service.CreateAsync("user-2", CreateRequest(price: 1), CancellationToken.None);

        var result = await service.GetThreeCheapestAsync("user-1", CancellationToken.None);

        Assert.Equal([10m, 20m, 30m], result.Select(vinyl => vinyl.Price));
    }

    private static CreateVinylDto CreateRequest(decimal price = 100) => new("Artist", "Album", 2020, "photo.jpg", price);
    private static UpdateVinylDto UpdateRequest() => new("Artist", "Album", 2021, "new.jpg", 150);
}

internal sealed class FakeVinylRepository : IVinylRepository
{
    public List<Vinyl> Items { get; } = [];

    public string? LastGetAllUserId { get; private set; }

    public CancellationToken LastGetAllCancellationToken { get; private set; }

    public Exception? GetAllException { get; init; }

    public string? LastCountUserId { get; private set; }

    public CancellationToken LastCountCancellationToken { get; private set; }

    public Exception? CountException { get; init; }

    public long LastPhotoOffset { get; private set; }

    public int LastPhotoPageSize { get; private set; }

    public string? LastPhotoUserId { get; private set; }

    public CancellationToken LastPhotoCancellationToken { get; private set; }

    public Task AddAsync(Vinyl vinyl, CancellationToken cancellationToken) { Items.Add(vinyl); return Task.CompletedTask; }

    public Task<IReadOnlyCollection<Vinyl>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        LastGetAllUserId = userId;

        LastGetAllCancellationToken = cancellationToken;

        if (GetAllException is not null) return Task.FromException<IReadOnlyCollection<Vinyl>>(GetAllException);

        return Task.FromResult<IReadOnlyCollection<Vinyl>>(Items.Where(v => v.Active && v.UserId == userId).ToList());
    }

    public Task<IReadOnlyCollection<Vinyl>> GetPhotosAsync(string userId, long offset, int pageSize, CancellationToken cancellationToken)
    {
        LastPhotoUserId = userId;

        LastPhotoCancellationToken = cancellationToken;

        LastPhotoOffset = offset;

        LastPhotoPageSize = pageSize;

        if (GetAllException is not null) return Task.FromException<IReadOnlyCollection<Vinyl>>(GetAllException);

        if (cancellationToken.IsCancellationRequested) return Task.FromCanceled<IReadOnlyCollection<Vinyl>>(cancellationToken);

        var vinyls = Items.Where(v => v.Active && v.UserId == userId)
            .OrderBy(v => v.Year)
            .ThenBy(v => v.Guid)
            .Where((_, index) => index >= offset)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<Vinyl>>(vinyls);
    }

    public Task<long> CountActiveAsync(string userId, CancellationToken cancellationToken)
    {
        LastCountUserId = userId;

        LastCountCancellationToken = cancellationToken;

        if (CountException is not null) return Task.FromException<long>(CountException);

        if (cancellationToken.IsCancellationRequested) return Task.FromCanceled<long>(cancellationToken);

        return Task.FromResult(Items.LongCount(v => v.Active && v.UserId == userId));
    }

    public Task<IReadOnlyCollection<Vinyl>> GetMostExpensiveAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Vinyl>>(Items.Where(v => v.Active && v.UserId == userId).OrderByDescending(v => v.Price).ThenBy(v => v.Guid).Take(3).ToList());

    public Task<IReadOnlyCollection<Vinyl>> GetThreeCheapestAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Vinyl>>(Items.Where(v => v.Active && v.UserId == userId).OrderBy(v => v.Price).ThenBy(v => v.Guid).Take(3).ToList());

    public Task<Vinyl?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken) => Task.FromResult(Items.SingleOrDefault(v => v.Guid == guid && v.UserId == userId && v.Active));

    public Task<bool> UpdateAsync(Vinyl vinyl, string userId, CancellationToken cancellationToken)
    {
        var item = Items.SingleOrDefault(v => v.Guid == vinyl.Guid && v.UserId == userId && v.Active);

        if (item is null) return Task.FromResult(false);

        item.Artist = vinyl.Artist;
        item.Album = vinyl.Album;
        item.Year = vinyl.Year;
        item.Photo = vinyl.Photo;
        item.Price = vinyl.Price;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken) { var item = Items.SingleOrDefault(v => v.Guid == guid && v.UserId == userId && v.Active); if (item is null) return Task.FromResult(false); item.Active = false; return Task.FromResult(true); }
}
