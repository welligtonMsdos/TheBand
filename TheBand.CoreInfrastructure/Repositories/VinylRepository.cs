using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;
using TheBand.CoreInfrastructure.Data;

namespace TheBand.CoreInfrastructure.Repositories;

public sealed class VinylRepository : IVinylRepository
{
    private readonly CoreContext _context;
    private readonly NpgsqlDataSource _dataSource;

    public VinylRepository(CoreContext context, NpgsqlDataSource dataSource)
    {
        _context = context;
        _dataSource = dataSource;
    }

    public async Task AddAsync(Vinyl vinyl, CancellationToken cancellationToken)
    {
        await _context.Vinyls.AddAsync(vinyl, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Vinyl>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Album", "Year", "Photo", "Price", "Active", "UserId"
            FROM "Vinyl"
            WHERE "Active" = TRUE AND "UserId" = @UserId
            ORDER By "Year";
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var vinyls = await connection.QueryAsync<Vinyl>(new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

        return vinyls.AsList();
    }

    public async Task<IReadOnlyCollection<Vinyl>> GetMostExpensiveAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Album", "Year", "Photo", "Price", "Active", "UserId"
            FROM "Vinyl"
            WHERE "Active" = TRUE AND "UserId" = @UserId
            ORDER BY "Price" DESC, "Guid"
            LIMIT 3;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var vinyls = await connection.QueryAsync<Vinyl>(new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

        return vinyls.AsList();
    }

    public async Task<IReadOnlyCollection<Vinyl>> GetThreeCheapestAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Album", "Year", "Photo", "Price", "Active", "UserId"
            FROM "Vinyl"
            WHERE "Active" = TRUE AND "UserId" = @UserId
            ORDER BY "Price", "Guid"
            LIMIT 3;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var vinyls = await connection.QueryAsync<Vinyl>(new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

        return vinyls.AsList();
    }

    public async Task<Vinyl?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Album", "Year", "Photo", "Price", "Active", "UserId"
            FROM "Vinyl"
            WHERE "Guid" = @Guid AND "UserId" = @UserId AND "Active" = TRUE;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Vinyl>(
            new CommandDefinition(sql, new { Guid = guid, UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Vinyl vinyl, string userId, CancellationToken cancellationToken)
    {
        var currentVinyl = await _context.Vinyls.FirstOrDefaultAsync(
            current => current.Guid == vinyl.Guid && current.UserId == userId && current.Active,
            cancellationToken);

        if (currentVinyl is null) return false;

        currentVinyl.Artist = vinyl.Artist;
        currentVinyl.Album = vinyl.Album;
        currentVinyl.Year = vinyl.Year;
        currentVinyl.Photo = vinyl.Photo;
        currentVinyl.Price = vinyl.Price;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        var vinyl = await _context.Vinyls.FirstOrDefaultAsync(v => v.Guid == guid && v.UserId == userId && v.Active, cancellationToken);

        if (vinyl is null) return false;

        vinyl.Active = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
