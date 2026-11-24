using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;

namespace TheBand.CoreApplication.Services;

public sealed class CollectionPathService : ICollectionPathService
{
    private readonly ICollectionPathRepository _repository;

    public CollectionPathService(ICollectionPathRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<CollectionPathDto>> GetActivePathsAsync(CancellationToken cancellationToken)
    {
        var paths = await _repository.GetActivePathsAsync(cancellationToken);

        return paths.Select(item => new CollectionPathDto(item.Path, item.Tag)).ToList();
    }
}
