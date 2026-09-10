using TheBand.AuthDomain.Entities;

namespace TheBand.AuthDomain.Interfaces;

public interface IUserRepository
{
    Task<ICollection<User>> GetUsersAsync();
    Task<User> GetDataLoginAsync(string Email);
}
