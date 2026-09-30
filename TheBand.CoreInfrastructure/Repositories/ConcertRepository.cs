using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheBand.CoreDomain.Entities;
using TheBand.CoreDomain.Interfaces;
using TheBand.CoreInfrastructure.Data;

namespace TheBand.CoreInfrastructure.Repositories;

public sealed class ConcertRepository : IConcertRepository
{
    private readonly CoreContext _context;
    private readonly NpgsqlDataSource _dataSource;

    public ConcertRepository(CoreContext context, NpgsqlDataSource dataSource)
    {
        _context = context;
        _dataSource = dataSource;
    }

    public async Task AddAsync(Concert concert, CancellationToken cancellationToken)
    {
        await _context.Concerts.AddAsync(concert, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Concert>> GetAllAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Venue", "ShowDate", "Photo", "Active", "UserId"
            FROM "Concert"
            WHERE "Active" = TRUE AND "UserId" = @UserId;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var concerts = await connection.QueryAsync<Concert>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

        return concerts.AsList();
    }

    public async Task<IReadOnlyCollection<Concert>> GetUpcomingAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Venue", "ShowDate", "Photo", "Active", "UserId"
            FROM "Concert"
            WHERE "Active" = TRUE AND "UserId" = @UserId AND "ShowDate" >= NOW();
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var concerts = await connection.QueryAsync<Concert>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

        return concerts.AsList();
    }

    public async Task<IReadOnlyCollection<Concert>> GetPastAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Venue", "ShowDate", "Photo", "Active", "UserId"
            FROM "Concert"
            WHERE "Active" = TRUE AND "UserId" = @UserId AND "ShowDate" < NOW();
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var concerts = await connection.QueryAsync<Concert>(
            new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken));

        return concerts.AsList();
    }

    public async Task<Concert?> GetByGuidAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT "Guid", "Artist", "Venue", "ShowDate", "Photo", "Active", "UserId"
            FROM "Concert"
            WHERE "Guid" = @Guid AND "UserId" = @UserId AND "Active" = TRUE;
            """;

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Concert>(
            new CommandDefinition(sql, new { Guid = guid, UserId = userId }, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Concert concert, string userId, CancellationToken cancellationToken)
    {
        var currentConcert = await _context.Concerts.FirstOrDefaultAsync(
            current => current.Guid == concert.Guid && current.UserId == userId && current.Active,
            cancellationToken);

        if (currentConcert is null) return false;

        currentConcert.Artist = concert.Artist;
        currentConcert.Venue = concert.Venue;
        currentConcert.ShowDate = concert.ShowDate;
        currentConcert.Photo = concert.Photo;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid guid, string userId, CancellationToken cancellationToken)
    {
        var concert = await _context.Concerts.FirstOrDefaultAsync(
            current => current.Guid == guid && current.UserId == userId && current.Active,
            cancellationToken);

        if (concert is null) return false;

        concert.Active = false;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
