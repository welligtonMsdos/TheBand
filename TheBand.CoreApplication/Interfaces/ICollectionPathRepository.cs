using TheBand.CoreApplication.Models;

namespace TheBand.CoreApplication.Interfaces;

public interface ICollectionPathRepository
{
    Task<IReadOnlyCollection<CollectionPathItem>> GetActivePathsAsync(string userId, CancellationToken cancellationToken);
}
