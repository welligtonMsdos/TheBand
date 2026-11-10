using TheBand.CoreApplication.Dtos;

namespace TheBand.CoreApplication.Interfaces;

public interface ICDService
{
    Task<CDDto> CreateAsync(string userId, CreateCDDto request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CDDto>> GetAllAsync(string userId, CancellationToken cancellationToken);

    Task<CDDto?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken);

    Task<CDDto?> UpdateAsync(Guid guid, string userId, UpdateCDDto request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken);
}
