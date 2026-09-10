using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Exceptions;
using TheBand.AuthApplication.Extensions;
using TheBand.AuthApplication.Interfaces;
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

    public async Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto)
    {
        var user = await _userRepository.GetDataLoginAsync(userLoginDto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
        {
            throw new BusinessException("Credenciais inválidas.");
        }

        var token = await _tokenService.GenerateToken(user.ToDataLoginDto());

        return new UserDataLoginDto
        (
            user._id,
            user.Name,
            user.Email,
            token,
            user.Role
        );
    }

    public async Task<ICollection<UserDto>> GetUsersAsync()
    {
        var user = await _userRepository.GetUsersAsync();

        ArgumentNullException.ThrowIfNull(user);

        return user
                .Select(e => e.ToUserDto())
                .ToList();
    }
}
