using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;

namespace TheBand.CoreApplication.Services;

public sealed class VinylService : IVinylService
{
    private readonly IVinylRepository _repository;

    public VinylService(IVinylRepository repository) => _repository = repository;

    public async Task<VinylDto> CreateAsync(string userId, CreateVinylDto request, CancellationToken cancellationToken)
    {
        ValidateUserId(userId);

        ArgumentNullException.ThrowIfNull(request);

        var vinyl = new Vinyl
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

        await _repository.AddAsync(vinyl, cancellationToken);

        return ToDto(vinyl);
    }

    public async Task<IReadOnlyCollection<VinylDto>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        ValidateUserId(userId);

        return (await _repository.GetAllAsync(userId, cancellationToken)).Select(ToDto).ToList();
    }

    public async Task<VinylDto?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        if (guid == Guid.Empty) throw new ArgumentException("O identificador do vinyl é inválido.", nameof(guid));

        ValidateUserId(userId);

        var vinyl = await _repository.GetByGuidAsync(guid, userId, cancellationToken);

        return vinyl is null ? null : ToDto(vinyl);
    }

    public async Task<VinylDto?> UpdateAsync(Guid guid, string userId, UpdateVinylDto request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (guid == Guid.Empty) throw new ArgumentException("O identificador do vinyl é inválido.", nameof(guid));

        ValidateUserId(userId);

        var vinyl = new Vinyl
        {
            Guid = guid, 
            Artist = request.Artist, 
            Album = request.Album, 
            Year = request.Year,
            Photo = request.Photo, 
            Price = request.Price, 
            UserId = userId, 
            Active = true
        };

        return await _repository.UpdateAsync(vinyl, userId, cancellationToken) ? ToDto(vinyl) : null;
    }

    public Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        if (guid == Guid.Empty) throw new ArgumentException("O identificador do vinyl é inválido.", nameof(guid));

        ValidateUserId(userId);

        return _repository.DeleteAsync(guid, userId, cancellationToken);
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("O identificador do usuário é obrigatório.", nameof(userId));
    }

    private static VinylDto ToDto(Vinyl vinyl) => new(vinyl.Guid, vinyl.Artist, vinyl.Album, vinyl.Year, vinyl.Photo, vinyl.Price);
}
