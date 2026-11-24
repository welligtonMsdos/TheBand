using Microsoft.EntityFrameworkCore;
using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreInfrastructure.Data;

namespace TheBand.CoreInfrastructure.Repositories;

public sealed class CollectionPathRepository : ICollectionPathRepository
{
    private readonly CoreContext _context;

    public CollectionPathRepository(CoreContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<CollectionPathDto>> GetActivePathsAsync(CancellationToken cancellationToken)
    {
        var cassettes = _context.Cassettes.AsNoTracking()
            .Where(item => item.Active)
            .Select(item => new { Path = item.Photo, Tag = "cassette" });

        var cds = _context.CDs.AsNoTracking()
            .Where(item => item.Active)
            .Select(item => new { Path = item.Photo, Tag = "cd" });

        var vinyls = _context.Vinyls.AsNoTracking()
            .Where(item => item.Active)
            .Select(item => new { Path = item.Photo, Tag = "vinyl" });

        var paths = await cassettes.Concat(cds).Concat(vinyls).ToListAsync(cancellationToken);

        return paths.Select(item => new CollectionPathDto(item.Path, item.Tag)).ToList();
    }
}
