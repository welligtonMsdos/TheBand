using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Interfaces;

public interface IUserService
{
    Task<ICollection<UserDto>> GetUsersAsync();
    Task<UserDataLoginDto> GetDataLoginAsync(UserLoginDto userLoginDto);
}
