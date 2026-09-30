using TheBand.CoreApplication.Dtos;

namespace TheBand.CoreApplication.Interfaces;

public interface ICassetteService
{
    Task<CassetteDto> CreateAsync(string userId, CreateCassetteDto request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CassetteDto>> GetAllAsync(string userId, CancellationToken cancellationToken);

    Task<CassetteDto?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken);

    Task<CassetteDto?> UpdateAsync(Guid guid, string userId, UpdateCassetteDto request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken);
}
