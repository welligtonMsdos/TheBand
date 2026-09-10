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

    public async Task<User> GetDataLoginAsync(string Email)
    {
        var user = await _context
                            .Users
                                .Find(u => u.Email == Email && u.Active)
                                .FirstOrDefaultAsync();

        return user;
    }

    public async Task<ICollection<User>> GetUsersAsync()
    {
        var users = await _context
                            .Users
                                .Find(u => u.Active)
                                .ToListAsync();

        return users;
    }
}
