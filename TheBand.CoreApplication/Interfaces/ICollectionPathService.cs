using TheBand.CoreApplication.Dtos;

namespace TheBand.CoreApplication.Interfaces;

public interface ICollectionPathService
{
    Task<IReadOnlyCollection<CollectionPathDto>> GetActivePathsAsync(string userId, CancellationToken cancellationToken);
}
