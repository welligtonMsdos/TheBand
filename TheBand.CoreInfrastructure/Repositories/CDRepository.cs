using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;
using TheBand.CoreInfrastructure.Data;

namespace TheBand.CoreInfrastructure.Repositories;

public class CDRepository : ICDRepository
{
    private readonly CoreContext _context;
    private readonly NpgsqlDataSource _dataSource;

    public CDRepository(CoreContext context, NpgsqlDataSource dataSource)
    {
        _context = context;
        _dataSource = dataSource;
    }

    public async Task AddAsync(CD cd, CancellationToken cancellationToken)
    {
        await _context.CDs.AddAsync(cd, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        var cd = await _context.CDs.FirstOrDefaultAsync(
            current => current.Guid == guid && current.UserId == userId && current.Active,
            cancellationToken);

        if (cd is null) return false;

        cd.Active = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyCollection<CD>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Album", "Year", "Photo", "Price", "Active", "UserId"
            FROM "CD"
            WHERE "Active" = TRUE AND "UserId" = @UserId;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var cds = await connection.QueryAsync<CD>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

        return cds.AsList();
    }

    public async Task<CD?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Album", "Year", "Photo", "Price", "Active", "UserId"
            FROM "CD"
            WHERE "Guid" = @Guid AND "UserId" = @UserId AND "Active" = TRUE;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<CD>(
            new CommandDefinition(sql, new { Guid = guid, UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(CD cd, string userId, CancellationToken cancellationToken)
    {
        var currentCD = await _context.CDs.FirstOrDefaultAsync(
            current => current.Guid == cd.Guid && current.UserId == userId && current.Active,
            cancellationToken);

        if (currentCD is null) return false;

        currentCD.Artist = cd.Artist;
        currentCD.Album = cd.Album;
        currentCD.Year = cd.Year;
        currentCD.Photo = cd.Photo;
        currentCD.Price = cd.Price;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
