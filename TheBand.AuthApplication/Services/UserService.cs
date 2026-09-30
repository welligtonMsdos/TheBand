using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Exceptions;
using TheBand.AuthApplication.Extensions;
using TheBand.AuthApplication.Interfaces;
using TheBand.AuthDomain.Entities;
using TheBand.AuthDomain.Interfaces;

namespace TheBand.AuthApplication.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetDataLoginAsync(NormalizeEmail(userLoginDto.Email), cancellationToken);

        if (user is null)
            throw new BusinessException("Usuário não encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
            throw new BusinessException("Senha inválida.");

        var token = await _tokenService.GenerateToken(user.ToDataLoginDto());

        return new UserDataLoginDto(user._id, user.Name, user.Email, token, user.Role);
    }

    public async Task<IReadOnlyCollection<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetUsersAsync(cancellationToken);

        return users.Select(user => user.ToUserDto()).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        return user?.ToUserDto();
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(createUserDto.Email);

        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (existingUser is not null)
            throw new BusinessException("Já existe um usuário cadastrado com este e-mail.");

        var user = new User
        {
            Name = createUserDto.Name.Trim(),
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            Role = createUserDto.Role,
            Active = true
        };

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        return createdUser.ToUserDto();
    }

    public async Task<UserDto?> UpdateAsync(string userId, UpdateUserDto updateUserDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            return null;

        var email = NormalizeEmail(updateUserDto.Email);

        var userWithSameEmail = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (userWithSameEmail is not null && userWithSameEmail._id != user._id)
            throw new BusinessException("Já existe um usuário cadastrado com este e-mail.");

        user.Name = updateUserDto.Name.Trim();
        user.Email = email;
        user.Role = updateUserDto.Role;        

        var updated = await _userRepository.UpdateAsync(user, cancellationToken);

        return updated ? user.ToUserDto() : null;
    }

    public Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default)
        => _userRepository.DeactivateAsync(userId, cancellationToken);

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
