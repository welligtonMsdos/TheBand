using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;
using TheBand.CoreInfrastructure.Data;

namespace TheBand.CoreInfrastructure.Repositories;

public sealed class CassetteRepository : ICassetteRepository
{
    private readonly CoreContext _context;
    private readonly NpgsqlDataSource _dataSource;

    public CassetteRepository(CoreContext context, NpgsqlDataSource dataSource)
    {
        _context = context;
        _dataSource = dataSource;
    }

    public async Task AddAsync(Cassette cassette, CancellationToken cancellationToken)
    {
        await _context.Cassettes.AddAsync(cassette, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Cassette>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Album", "Year", "Photo", "Price", "Active", "UserId"
            FROM "Cassette"
            WHERE "Active" = TRUE AND "UserId" = @UserId;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var cassettes = await connection.QueryAsync<Cassette>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

        return cassettes.AsList();
    }

    public async Task<Cassette?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Album", "Year", "Photo", "Price", "Active", "UserId"
            FROM "Cassette"
            WHERE "Guid" = @Guid AND "UserId" = @UserId AND "Active" = TRUE;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Cassette>(
            new CommandDefinition(sql, new { Guid = guid, UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Cassette cassette, string userId, CancellationToken cancellationToken)
    {
        var currentCassette = await _context.Cassettes.FirstOrDefaultAsync(
            current => current.Guid == cassette.Guid && current.UserId == userId && current.Active,
            cancellationToken);

        if (currentCassette is null) return false;

        currentCassette.Artist = cassette.Artist;
        currentCassette.Album = cassette.Album;
        currentCassette.Year = cassette.Year;
        currentCassette.Photo = cassette.Photo;
        currentCassette.Price = cassette.Price;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        var cassette = await _context.Cassettes.FirstOrDefaultAsync(
            current => current.Guid == guid && current.UserId == userId && current.Active,
            cancellationToken);

        if (cassette is null) return false;

        cassette.Active = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
