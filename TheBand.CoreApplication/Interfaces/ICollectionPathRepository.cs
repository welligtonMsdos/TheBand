using TheBand.CoreApplication.Dtos;

namespace TheBand.CoreApplication.Interfaces;

public interface ICollectionPathRepository
{
    Task<IReadOnlyCollection<CollectionPathDto>> GetActivePathsAsync(CancellationToken cancellationToken);
}
