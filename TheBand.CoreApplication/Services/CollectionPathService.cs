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

    public async Task<IReadOnlyCollection<CollectionPathDto>> GetActivePathsAsync(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("O identificador do usuário é obrigatório.", nameof(userId));
        }

        var paths = await _repository.GetActivePathsAsync(userId, cancellationToken);

        return paths.Select(item => new CollectionPathDto(item.Path, item.Tag)).ToList();
    }
}
