using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Services;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;

namespace TheBand.CoreTests;

public sealed class CassetteServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsCassetteForUser()
    {
        var repository = new FakeCassetteRepository();
        var service = new CassetteService(repository);

        var result = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Guid);
        Assert.Single(repository.Items);
        Assert.Equal("user-1", repository.Items.Single().UserId);
        Assert.True(repository.Items.Single().Active);
    }

    // [Fact]
    // public async Task GetAllAsync_DifferentUser_DoesNotReturnAnotherUsersCassette()
    // {
    //     var repository = new FakeCassetteRepository();
    //     var service = new CassetteService(repository);

    //     await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

    //     Assert.Empty(await service.GetAllAsync("user-2", CancellationToken.None));
    // }

    [Fact]
    public async Task UpdateAndDeleteAsync_ExistingCassette_UpdatesThenHidesCassette()
    {
        var repository = new FakeCassetteRepository();
        var service = new CassetteService(repository);
        var created = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        var updated = await service.UpdateAsync(created.Guid, "user-1", UpdateRequest(), CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(2021, updated.Year);
        Assert.True(await service.DeleteAsync(created.Guid, "user-1", CancellationToken.None));
        Assert.Null(await service.GetByGuidAsync(created.Guid, "user-1", CancellationToken.None));
    }

    [Fact]
    public async Task GetByGuidAsync_MissingCassette_ReturnsNull()
    {
        var service = new CassetteService(new FakeCassetteRepository());

        Assert.Null(await service.GetByGuidAsync(Guid.NewGuid(), "user-1", CancellationToken.None));
    }

    private static CreateCassetteDto CreateRequest() => new("Artist", "Album", 2020, "https://photo.jpg", 100);

    private static UpdateCassetteDto UpdateRequest() => new("Artist", "Album", 2021, "https://new-photo.jpg", 150);
}

internal sealed class FakeCassetteRepository : ICassetteRepository
{
    public List<Cassette> Items { get; } = [];

    public Task AddAsync(Cassette cassette, CancellationToken cancellationToken)
    {
        Items.Add(cassette);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Cassette>> GetAllAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Cassette>>(Items.Where(cassette => cassette.Active && cassette.UserId == userId).ToList());

    public Task<Cassette?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken) =>
        Task.FromResult(Items.SingleOrDefault(cassette => cassette.Guid == guid && cassette.UserId == userId && cassette.Active));

    public Task<bool> UpdateAsync(Cassette cassette, string userId, CancellationToken cancellationToken)
    {
        var item = Items.SingleOrDefault(current => current.Guid == cassette.Guid && current.UserId == userId && current.Active);

        if (item is null) return Task.FromResult(false);

        item.Artist = cassette.Artist;
        item.Album = cassette.Album;
        item.Year = cassette.Year;
        item.Photo = cassette.Photo;
        item.Price = cassette.Price;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        var item = Items.SingleOrDefault(cassette => cassette.Guid == guid && cassette.UserId == userId && cassette.Active);

        if (item is null) return Task.FromResult(false);

        item.Active = false;

        return Task.FromResult(true);
    }
}
