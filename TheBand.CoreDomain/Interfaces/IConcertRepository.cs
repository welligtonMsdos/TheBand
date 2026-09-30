using TheBand.CoreDomain.Entities;

namespace TheBand.CoreDomain.Interfaces;

public interface IConcertRepository
{
    Task AddAsync(Concert concert, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Concert>> GetAllAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Concert>> GetUpcomingAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Concert>> GetPastAsync(string userId, CancellationToken cancellationToken);

    Task<Concert?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Concert concert, string userId, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken);
}
