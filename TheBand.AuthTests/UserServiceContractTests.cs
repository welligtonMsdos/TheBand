using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Exceptions;
using TheBand.AuthApplication.Interfaces;
using TheBand.AuthApplication.Services;
using TheBand.AuthDomain.Entities;
using TheBand.AuthDomain.Enum;
using TheBand.AuthDomain.Interfaces;

namespace TheBand.AuthTests;

public sealed class UserServiceContractTests
{
    [Fact]
    public async Task IUserService_GetDataLoginAsync_ValidCredentials_ReturnsTokenData()
    {
        var repository = new FakeUserRepository { LoginUser = CreateUser() };
        IUserService service = new UserService(repository, new FakeTokenService("generated-token"));

        var result = await service.GetDataLoginAsync(new UserLoginDto("MARIA@example.com", "SenhaSegura123"));

        Assert.Equal("generated-token", result.Token);
        Assert.Equal(repository.LoginUser._id, result._id);
        Assert.Equal("maria@example.com", repository.LastLoginEmail);
    }

    [Fact]
    public async Task IUserService_CreateAsync_CreatesActiveUserWithHashedPassword()
    {
        var repository = new FakeUserRepository();
        IUserService service = new UserService(repository, new FakeTokenService("token"));

        var result = await service.CreateAsync(new CreateUserDto("Maria Silva", "MARIA@example.com", "SenhaSegura123"));

        Assert.Equal("maria@example.com", result.Email);
        Assert.True(result.Active);
        Assert.NotNull(repository.CreatedUser);
        Assert.True(BCrypt.Net.BCrypt.Verify("SenhaSegura123", repository.CreatedUser!.Password));
    }

    [Fact]
    public async Task IUserService_CreateAsync_DuplicateEmail_ThrowsBusinessException()
    {
        var repository = new FakeUserRepository { EmailUser = CreateUser() };
        IUserService service = new UserService(repository, new FakeTokenService("token"));

        await Assert.ThrowsAsync<BusinessException>(() => service.CreateAsync(new CreateUserDto("Outra Pessoa", "maria@example.com", "SenhaSegura123")));
    }

    [Fact]
    public async Task IUserService_GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        IUserService service = new UserService(new FakeUserRepository(), new FakeTokenService("token"));

        var result = await service.GetByIdAsync("missing");

        Assert.Null(result);
    }

    [Fact]
    public async Task IUserService_UpdateAsync_WithoutPassword_PreservesExistingPassword()
    {
        var user = CreateUser();
        var repository = new FakeUserRepository { IdUser = user, EmailUser = user };
        IUserService service = new UserService(repository, new FakeTokenService("token"));

        var result = await service.UpdateAsync(user._id, new UpdateUserDto("Maria Atualizada", "maria@example.com", null, UserRole.Admin));

        Assert.NotNull(result);
        Assert.Equal("Maria Atualizada", result.Name);
        Assert.Equal(UserRole.Admin, result.Role);
        Assert.Equal(user.Password, repository.UpdatedUser!.Password);
    }

    [Fact]
    public async Task IUserService_DeleteAsync_DeactivatesUser()
    {
        var repository = new FakeUserRepository { DeactivateResult = true };
        IUserService service = new UserService(repository, new FakeTokenService("token"));

        var deleted = await service.DeleteAsync("66f0f0aabf1c2d3e4f5a6b7c");

        Assert.True(deleted);
        Assert.Equal("66f0f0aabf1c2d3e4f5a6b7c", repository.DeactivatedUserId);
    }

    private static User CreateUser() => new()
    {
        _id = "66f0f0aabf1c2d3e4f5a6b7c",
        Name = "Maria Silva",
        Email = "maria@example.com",
        Password = BCrypt.Net.BCrypt.HashPassword("SenhaSegura123"),
        Role = UserRole.User,
        Active = true
    };

    private sealed class FakeUserRepository : IUserRepository
    {
        public User? LoginUser { get; init; }
        public User? IdUser { get; init; }
        public User? EmailUser { get; init; }
        public User? CreatedUser { get; private set; }
        public User? UpdatedUser { get; private set; }
        public bool DeactivateResult { get; init; }
        public string? LastLoginEmail { get; private set; }
        public string? DeactivatedUserId { get; private set; }

        public Task<IReadOnlyCollection<User>> GetUsersAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<User>>([]);

        public Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default) => Task.FromResult(IdUser);

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => Task.FromResult(EmailUser);

        public Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            user._id = "66f0f0aabf1c2d3e4f5a6b7c";
            CreatedUser = user;
            return Task.FromResult(user);
        }

        public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            UpdatedUser = user;
            return Task.FromResult(true);
        }

        public Task<bool> DeactivateAsync(string userId, CancellationToken cancellationToken = default)
        {
            DeactivatedUserId = userId;
            return Task.FromResult(DeactivateResult);
        }

        public Task<User?> GetDataLoginAsync(string email, CancellationToken cancellationToken = default)
        {
            LastLoginEmail = email;
            return Task.FromResult(LoginUser);
        }
    }

    private sealed class FakeTokenService(string token) : ITokenService
    {
        public Task<string> GenerateToken(UserDataLoginDto userDataLoginDto) => Task.FromResult(token);
    }
}
