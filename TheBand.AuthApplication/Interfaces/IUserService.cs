using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Interfaces;

public interface IUserService
{
    Task<IReadOnlyCollection<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<UserDto?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<UserDto> CreateAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default);

    Task<UserDto?> UpdateAsync(string userId, UpdateUserDto updateUserDto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default);

    Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken = default);
}
