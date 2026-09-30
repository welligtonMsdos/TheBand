using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;

namespace TheBand.CoreApplication.Services;

public sealed class ConcertService : IConcertService
{
    private readonly IConcertRepository _repository;

    public ConcertService(IConcertRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConcertDto> CreateAsync(string userId, CreateConcertDto request, CancellationToken cancellationToken)
    {
        ValidateUserId(userId);

        ArgumentNullException.ThrowIfNull(request);

        var concert = new Concert
        {
            Guid = Guid.NewGuid(),
            Artist = request.Artist,
            Venue = request.Venue,
            ShowDate = request.ShowDate,
            Photo = request.Photo,
            Active = true,
            UserId = userId
        };

        await _repository.AddAsync(concert, cancellationToken);

        return ToDto(concert);
    }

    public async Task<IReadOnlyCollection<ConcertDto>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        ValidateUserId(userId);

        return (await _repository.GetAllAsync(userId, cancellationToken)).Select(ToDto).ToList();
    }

    public async Task<ConcertDto?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        ValidateGuid(guid);

        ValidateUserId(userId);

        var concert = await _repository.GetByGuidAsync(guid, userId, cancellationToken);

        return concert is null ? null : ToDto(concert);
    }

    public async Task<ConcertDto?> UpdateAsync(Guid guid, string userId, UpdateConcertDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateGuid(guid);

        ValidateUserId(userId);

        var concert = new Concert
        {
            Guid = guid,
            Artist = request.Artist,
            Venue = request.Venue,
            ShowDate = request.ShowDate,
            Photo = request.Photo,
            Active = true,
            UserId = userId
        };

        return await _repository.UpdateAsync(concert, userId, cancellationToken) ? ToDto(concert) : null;
    }

    public Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        ValidateGuid(guid);

        ValidateUserId(userId);

        return _repository.DeleteAsync(guid, userId, cancellationToken);
    }

    private static void ValidateGuid(Guid guid)
    {
        if (guid == Guid.Empty)
            throw new ArgumentException("O identificador do show é inválido.", nameof(guid));
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("O identificador do usuário é obrigatório.", nameof(userId));
    }

    private static ConcertDto ToDto(Concert concert) => new(
        concert.Guid,
        concert.Artist,
        concert.Venue,
        concert.ShowDate,
        concert.Photo);
}
