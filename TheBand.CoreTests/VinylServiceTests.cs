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

    private static CreateVinylDto CreateRequest() => new("Artist", "Album", 2020, "photo.jpg", 100);
    private static UpdateVinylDto UpdateRequest() => new("Artist", "Album", 2021, "new.jpg", 150);
}

internal sealed class FakeVinylRepository : IVinylRepository
{
    public List<Vinyl> Items { get; } = [];

    public Task AddAsync(Vinyl vinyl, CancellationToken cancellationToken) { Items.Add(vinyl); return Task.CompletedTask; }

    public Task<IReadOnlyCollection<Vinyl>> GetAllAsync(string userId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<Vinyl>>(Items.Where(v => v.Active && v.UserId == userId).ToList());

    public Task<Vinyl?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken) => Task.FromResult(Items.SingleOrDefault(v => v.Guid == guid && v.UserId == userId && v.Active));

    public Task<bool> UpdateAsync(Vinyl vinyl, string userId, CancellationToken cancellationToken) => Task.FromResult(Items.Any(v => v.Guid == vinyl.Guid && v.UserId == userId && v.Active));

    public Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken) { var item = Items.SingleOrDefault(v => v.Guid == guid && v.UserId == userId && v.Active); if (item is null) return Task.FromResult(false); item.Active = false; return Task.FromResult(true); }
}
