using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreApplication.Services;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;

namespace TheBand.CoreTests;

public sealed class ConcertServiceTests
{
    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsAndReturnsConcert()
    {
        var repository = new FakeConcertRepository();
        var service = new ConcertService(repository);

        var result = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Guid);
        Assert.Equal(new DateOnly(2026, 10, 1), result.ShowDate);
        Assert.Single(repository.Items);
        Assert.True(repository.Items.Single().Active);
    }

    [Fact]
    public async Task GetByGuidAsync_MissingConcert_ReturnsNull()
    {
        var service = new ConcertService(new FakeConcertRepository());

        Assert.Null(await service.GetByGuidAsync(Guid.NewGuid(), "user-1", CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ExistingConcert_UpdatesOnlyTheOwnersConcert()
    {
        var repository = new FakeConcertRepository();
        var service = new ConcertService(repository);
        var created = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        var updated = await service.UpdateAsync(created.Guid, "user-1", UpdateRequest(), CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("New Venue", updated.Venue);
        Assert.Equal(new DateOnly(2026, 11, 1), updated.ShowDate);
    }

    [Fact]
    public async Task GetAllAsync_DifferentUser_DoesNotReturnAnotherUsersConcert()
    {
        var repository = new FakeConcertRepository();
        var service = new ConcertService(repository);

        await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        Assert.Empty(await service.GetAllAsync("user-2", CancellationToken.None));
    }

    [Fact]
    public async Task GetUpcomingAndPastAsync_ReturnOnlyTheUsersConcertsInTheRequestedPeriod()
    {
        var repository = new FakeConcertRepository();
        var service = new ConcertService(repository);
        var tomorrow = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        var yesterday = tomorrow.AddDays(-2);

        repository.Items.AddRange([
            CreateConcert("user-1", tomorrow),
            CreateConcert("user-1", yesterday),
            CreateConcert("user-2", tomorrow.AddDays(1))
        ]);

        var upcoming = await service.GetUpcomingAsync("user-1", CancellationToken.None);
        var past = await service.GetPastAsync("user-1", CancellationToken.None);

        Assert.Single(upcoming);
        Assert.Equal(tomorrow, upcoming.Single().ShowDate);
        Assert.Single(past);
        Assert.Equal(yesterday, past.Single().ShowDate);
    }

    [Fact]
    public async Task DeleteAsync_ExistingConcert_ReturnsTrueAndHidesIt()
    {
        var repository = new FakeConcertRepository();
        var service = new ConcertService(repository);
        var created = await service.CreateAsync("user-1", CreateRequest(), CancellationToken.None);

        Assert.True(await service.DeleteAsync(created.Guid, "user-1", CancellationToken.None));
        Assert.Null(await service.GetByGuidAsync(created.Guid, "user-1", CancellationToken.None));
    }

    [Fact]
    public async Task GetAllAsync_EmptyUserId_ThrowsArgumentException()
    {
        var service = new ConcertService(new FakeConcertRepository());

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetAllAsync(string.Empty, CancellationToken.None));
    }

    [Fact]
    public async Task GetPriceByYearAsync_GroupsAndSumsActiveConcertsForTheUserInYearOrder()
    {
        var repository = new FakeConcertRepository();

        var first = CreateConcert("user-1", new DateOnly(2026, 1, 1));

        first.Price = 125.50m;

        var second = CreateConcert("user-1", new DateOnly(2026, 12, 31));

        second.Price = 74.75m;

        var previousYear = CreateConcert("user-1", new DateOnly(2025, 12, 31));

        previousYear.Price = 50m;

        var inactive = CreateConcert("user-1", new DateOnly(2024, 1, 1));

        inactive.Active = false;

        repository.Items.AddRange([
            first,
            second,
            previousYear,
            inactive,
            CreateConcert("user-2", new DateOnly(2026, 1, 1)),
            CreateConcert("user-2", new DateOnly(2023, 1, 1))
        ]);

        IConcertService service = new ConcertService(repository);

        var result = await service.GetPriceByYearAsync("user-1", CancellationToken.None);

        Assert.Equal([
            new ConcertPriceByYearDto(2025, 50m),
            new ConcertPriceByYearDto(2026, 200.25m)
        ], result);
    }

    [Fact]
    public async Task GetPriceByYearAsync_NoConcerts_ReturnsEmptyCollection()
    {
        IConcertService service = new ConcertService(new FakeConcertRepository());

        Assert.Empty(await service.GetPriceByYearAsync("user-1", CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetPriceByYearAsync_InvalidUserId_ThrowsArgumentException(string? userId)
    {
        IConcertService service = new ConcertService(new FakeConcertRepository());

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetPriceByYearAsync(userId!, CancellationToken.None));
    }

    [Fact]
    public async Task GetPriceByYearAsync_ForwardsCancellationTokenToRepository()
    {
        using var cancellation = new CancellationTokenSource();

        var repository = new FakeConcertRepository();

        IConcertService service = new ConcertService(repository);

        await service.GetPriceByYearAsync("user-1", cancellation.Token);

        Assert.Equal(cancellation.Token, repository.LastGetAllCancellationToken);
    }

    [Fact]
    public async Task GetPriceByYearAsync_RepositoryFailure_PropagatesException()
    {
        var exception = new InvalidOperationException("Repository unavailable.");

        var repository = new FakeConcertRepository { GetAllException = exception };

        IConcertService service = new ConcertService(repository);

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetPriceByYearAsync("user-1", CancellationToken.None));

        Assert.Same(exception, result);
    }

    private static CreateConcertDto CreateRequest() => new("Artist", "Venue", new DateOnly(2026, 10, 1), "photo.jpg", 1);

    private static UpdateConcertDto UpdateRequest() => new("Artist", "New Venue", new DateOnly(2026, 11, 1), "new.jpg", 1);

    private static Concert CreateConcert(string userId, DateOnly showDate) => new()
    {
        Guid = Guid.NewGuid(),
        Artist = "Artist",
        Venue = "Venue",
        ShowDate = showDate,
        Photo = "photo.jpg",
        Price = 1,
        Active = true,
        UserId = userId
    };
}

internal sealed class FakeConcertRepository : IConcertRepository
{
    public List<Concert> Items { get; } = [];

    public CancellationToken LastGetAllCancellationToken { get; private set; }

    public Exception? GetAllException { get; init; }

    public Task AddAsync(Concert concert, CancellationToken cancellationToken)
    {
        Items.Add(concert);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Concert>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        LastGetAllCancellationToken = cancellationToken;

        if (GetAllException is not null)
            return Task.FromException<IReadOnlyCollection<Concert>>(GetAllException);

        return Task.FromResult<IReadOnlyCollection<Concert>>(Items.Where(item => item.Active && item.UserId == userId).ToList());
    }

    public Task<IReadOnlyCollection<Concert>> GetUpcomingAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Concert>>(Items.Where(item => item.Active && item.UserId == userId && item.ShowDate >= DateOnly.FromDateTime(DateTime.Today)).ToList());

    public Task<IReadOnlyCollection<Concert>> GetPastAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Concert>>(Items.Where(item => item.Active && item.UserId == userId && item.ShowDate < DateOnly.FromDateTime(DateTime.Today)).ToList());

    public Task<Concert?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken) =>
        Task.FromResult(Items.SingleOrDefault(item => item.Guid == guid && item.UserId == userId && item.Active));

    public Task<bool> UpdateAsync(Concert concert, string userId, CancellationToken cancellationToken)
    {
        var item = Items.SingleOrDefault(current => current.Guid == concert.Guid && current.UserId == userId && current.Active);

        if (item is null) return Task.FromResult(false);

        item.Artist = concert.Artist;
        item.Venue = concert.Venue;
        item.ShowDate = concert.ShowDate;
        item.Photo = concert.Photo;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        var item = Items.SingleOrDefault(current => current.Guid == guid && current.UserId == userId && current.Active);

        if (item is null) return Task.FromResult(false);

        item.Active = false;

        return Task.FromResult(true);
    }
}
