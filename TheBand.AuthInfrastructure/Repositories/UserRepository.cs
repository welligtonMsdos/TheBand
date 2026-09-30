using MongoDB.Bson;
using MongoDB.Driver;
using TheBand.AuthDomain.Entities;
using TheBand.AuthDomain.Interfaces;
using TheBand.AuthInfrastructure.Data;

namespace TheBand.AuthInfrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthContext _context;

    public UserRepository(AuthContext context)
    {
        _context = context;
    }

    public async Task<User?> GetDataLoginAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(user => user.Email == email && user.Active)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(user => user.Active)
            .SortBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out _))
            return null;

        return await _context.Users
            .Find(user => user._id == userId && user.Active)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Find(user => user.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.InsertOneAsync(user, cancellationToken: cancellationToken);

        return user;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var result = await _context.Users.ReplaceOneAsync(
            currentUser => currentUser._id == user._id && currentUser.Active,
            user,
            cancellationToken: cancellationToken);

        return result.MatchedCount == 1;
    }

    public async Task<bool> DeactivateAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(userId, out _))
            return false;

        var update = Builders<User>.Update.Set(user => user.Active, false);

        var result = await _context.Users.UpdateOneAsync(
            user => user._id == userId && user.Active,
            update,
            cancellationToken: cancellationToken);

        return result.ModifiedCount == 1;
    }
}
