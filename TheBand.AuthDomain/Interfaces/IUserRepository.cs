using TheBand.AuthDomain.Entities;

namespace TheBand.AuthDomain.Interfaces;

public interface IUserRepository
{
    Task<IReadOnlyCollection<User>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(string userId, CancellationToken cancellationToken = default);

    Task<User?> GetDataLoginAsync(string email, CancellationToken cancellationToken = default);
}
