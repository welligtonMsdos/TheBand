using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Services;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;

namespace TheBand.CoreTests;

public sealed class CDServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsCassetteForUser()
    {
        var repository = new FakeCDRepository();
        var service = new CDService(repository);

        var result = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Guid);
        Assert.Single(repository.Items);
        Assert.Equal("user-1", repository.Items.Single().UserId);
        Assert.True(repository.Items.Single().Active);
    }

    [Fact]
    public async Task GetAllAsync_DifferentUser_DoesNotReturnAnotherUsersCassette()
    {
        var repository = new FakeCDRepository();
        var service = new CDService(repository);

        await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        Assert.Empty(await service.GetAllAsync("user-2", CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAndDeleteAsync_ExistingCassette_UpdatesThenHidesCassette()
    {
        var repository = new FakeCDRepository();
        var service = new CDService(repository);
        var created = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        var updated = await service.UpdateAsync(created.Guid, "user-1", UpdateRequest(), CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(2021, updated.Year);
        Assert.True(await service.DeleteAsync(created.Guid, "user-1", CancellationToken.None));
        Assert.Null(await service.GetByGuidAsync(created.Guid, "user-1", CancellationToken.None));
    }

    [Fact]
    public async Task GetByGuidAsync_MissingCD_ReturnsNull()
    {
        var service = new CDService(new FakeCDRepository());

        Assert.Null(await service.GetByGuidAsync(Guid.NewGuid(), "user-1", CancellationToken.None));
    }

    private static CreateCDDto CreateRequest() => new("Artist", "Album", 2020, "https://photo.jpg", 100);

    private static UpdateCDDto UpdateRequest() => new("Artist", "Album", 2021, "https://new-photo.jpg", 150);
}

internal sealed class FakeCDRepository : ICDRepository
{
    public List<CD> Items { get; } = [];

    public Task AddAsync(CD cd, CancellationToken cancellationToken)
    {
        Items.Add(cd);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<CD>> GetAllAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<CD>>(Items.Where(cd => cd.Active && cd.UserId == userId).ToList());

    public Task<CD?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken) =>
        Task.FromResult(Items.SingleOrDefault(cd => cd.Guid == guid && cd.UserId == userId && cd.Active));

    public Task<bool> UpdateAsync(CD cd, string userId, CancellationToken cancellationToken)
    {
        var item = Items.SingleOrDefault(current => current.Guid == cd.Guid && current.UserId == userId && current.Active);

        if (item is null) return Task.FromResult(false);

        item.Artist = cd.Artist;
        item.Album = cd.Album;
        item.Year = cd.Year;
        item.Photo = cd.Photo;
        item.Price = cd.Price;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        var item = Items.SingleOrDefault(cd => cd.Guid == guid && cd.UserId == userId && cd.Active);

        if (item is null) return Task.FromResult(false);

        item.Active = false;

        return Task.FromResult(true);
    }
}
