using TheBand.CoreDomain.Entities;

namespace TheBand.CoreDomain.Interfaces;

public interface ICassetteRepository
{
    Task AddAsync(Cassette cassette, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Cassette>> GetAllAsync(string userId, CancellationToken cancellationToken);

    Task<Cassette?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Cassette cassette, string userId, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken);
}
