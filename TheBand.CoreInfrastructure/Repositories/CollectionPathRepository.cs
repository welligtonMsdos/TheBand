using Microsoft.EntityFrameworkCore;
using TheBand.CoreApplication.Interfaces;
using TheBand.CoreApplication.Models;
using TheBand.CoreInfrastructure.Data;

namespace TheBand.CoreInfrastructure.Repositories;

public sealed class CollectionPathRepository : ICollectionPathRepository
{
    private readonly CoreContext _context;

    public CollectionPathRepository(CoreContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<CollectionPathItem>> GetActivePathsAsync(string userId, CancellationToken cancellationToken)
    {
        var cassettes = _context.Cassettes.AsNoTracking()
            .Where(item => item.Active && item.UserId == userId)
            .Select(item => new { Path = item.Photo, Tag = "cassette" });

        var cds = _context.CDs.AsNoTracking()
            .Where(item => item.Active && item.UserId == userId)
            .Select(item => new { Path = item.Photo, Tag = "cd" });

        var vinyls = _context.Vinyls.AsNoTracking()
            .Where(item => item.Active && item.UserId == userId)
            .Select(item => new { Path = item.Photo, Tag = "vinyl" });

        var paths = await cassettes.Concat(cds).Concat(vinyls).ToListAsync(cancellationToken);

        return paths.Select(item => new CollectionPathItem(item.Path, item.Tag)).ToList();
    }
}
