using TheBand.CoreDomain.Entities;

namespace TheBand.CoreDomain.Interfaces;

public interface ICDRepository
{
    Task AddAsync(CD cd, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CD>> GetAllAsync(string userId, CancellationToken cancellationToken);

    Task<CD?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(CD cd, string userId, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken);
}
