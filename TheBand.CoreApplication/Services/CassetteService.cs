using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;

namespace TheBand.CoreApplication.Services;

public sealed class CassetteService : ICassetteService
{
    private readonly ICassetteRepository _repository;

    public CassetteService(ICassetteRepository repository)
    {
        _repository = repository;
    }

    public async Task<CassetteDto> CreateAsync(string userId, CreateCassetteDto request, CancellationToken cancellationToken)
    {
        ValidateUserId(userId);

        ArgumentNullException.ThrowIfNull(request);

        var cassette = new Cassette
        {
            Guid = Guid.NewGuid(),
            Artist = request.Artist,
            Album = request.Album,
            Year = request.Year,
            Photo = request.Photo,
            Price = request.Price,
            Active = true,
            UserId = userId
        };

        await _repository.AddAsync(cassette, cancellationToken);

        return ToDto(cassette);
    }

    public async Task<IReadOnlyCollection<CassetteDto>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        ValidateUserId(userId);

        return (await _repository.GetAllAsync(userId, cancellationToken)).Select(ToDto).ToList();
    }

    public async Task<CassetteDto?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        ValidateGuid(guid);

        ValidateUserId(userId);

        var cassette = await _repository.GetByGuidAsync(guid, userId, cancellationToken);

        return cassette is null ? null : ToDto(cassette);
    }

    public async Task<CassetteDto?> UpdateAsync(Guid guid, string userId, UpdateCassetteDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateGuid(guid);

        ValidateUserId(userId);

        var cassette = new Cassette
        {
            Guid = guid,
            Artist = request.Artist,
            Album = request.Album,
            Year = request.Year,
            Photo = request.Photo,
            Price = request.Price,
            Active = true,
            UserId = userId
        };

        return await _repository.UpdateAsync(cassette, userId, cancellationToken) ? ToDto(cassette) : null;
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
            throw new ArgumentException("O identificador da fita cassete é inválido.", nameof(guid));
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("O identificador do usuário é obrigatório.", nameof(userId));
    }

    private static CassetteDto ToDto(Cassette cassette) => new(
        cassette.Guid,
        cassette.Artist,
        cassette.Album,
        cassette.Year,
        cassette.Photo,
        cassette.Price);
}
