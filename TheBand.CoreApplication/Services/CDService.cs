using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;

namespace TheBand.CoreApplication.Services;

public class CDService : ICDService
{
    private readonly ICDRepository _repository;

    public CDService(ICDRepository repository)
    {
        _repository = repository;
    }

    public async Task<CDDto> CreateAsync(
        string userId, 
        CreateCDDto request, 
        CancellationToken cancellationToken)
    {
        ValidateUserId(userId);

        ArgumentNullException.ThrowIfNull(request);

        var cd = new CD
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

        await _repository.AddAsync(cd, cancellationToken);

        return ToDto(cd);
    }

    public async Task<bool> DeleteAsync(
        Guid guid, 
        string userId, 
        CancellationToken cancellationToken)
    {
        ValidateGuid(guid);

        ValidateUserId(userId);

        return await _repository.DeleteAsync(
            guid, 
            userId, 
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<CDDto>> GetAllAsync(
        string userId, 
        CancellationToken cancellationToken)
    {
        ValidateUserId(userId);

        return (await _repository.GetAllAsync(
            userId, 
            cancellationToken)).Select(ToDto)
                               .ToList();
    }

    public async Task<CDDto?> GetByGuidAsync(
        Guid guid, 
        string userId, 
        CancellationToken cancellationToken)
    {
        ValidateGuid(guid);

        ValidateUserId(userId);

        var cd = await _repository.GetByGuidAsync(
            guid, 
            userId, 
            cancellationToken);

        return cd is null ? null : ToDto(cd);
    }

    public async Task<CDDto?> UpdateAsync(
        Guid guid, 
        string userId, 
        UpdateCDDto request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateGuid(guid);

        ValidateUserId(userId);

        var cd = new CD
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

        return await _repository.UpdateAsync(
            cd, 
            userId, 
            cancellationToken) ? ToDto(cd) : null;
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

    private static CDDto ToDto(CD cd) => new(
        cd.Guid,
        cd.Artist,
        cd.Album,
        cd.Year,
        cd.Photo,
        cd.Price);
}
