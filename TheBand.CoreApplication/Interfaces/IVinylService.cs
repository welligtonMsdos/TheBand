using TheBand.CoreApplication.Dtos;

namespace TheBand.CoreApplication.Interfaces;

public interface IVinylService
{
    Task<VinylDto> CreateAsync(string userId, CreateVinylDto request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<VinylDto>> GetAllAsync(string userId, CancellationToken cancellationToken);

    Task<VinylDto?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken);

    Task<VinylDto?> UpdateAsync(Guid guid, string userId, UpdateVinylDto request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken);
}
