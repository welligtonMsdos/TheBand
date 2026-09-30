using TheBand.CoreDomain.Entities;

namespace TheBand.CoreDomain.Interfaces;

public interface IVinylRepository
{
    Task AddAsync(Vinyl vinyl, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Vinyl>> GetAllAsync(string userId, CancellationToken cancellationToken);

    Task<Vinyl?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Vinyl vinyl, string userId, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken);
}
