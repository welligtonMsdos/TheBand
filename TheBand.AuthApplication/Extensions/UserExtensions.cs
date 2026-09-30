using TheBand.AuthApplication.Dtos;
using TheBand.AuthDomain.Entities;

namespace TheBand.AuthApplication.Extensions;

public static class UserExtensions
{
    public static UserDto ToUserDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDto(user._id,
                           user.Name,
                           user.Email,
                           user.Role,
                           user.Active);
    }

    public static UserLoginDto ToLoginDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserLoginDto(user.Email,
                                user.Password);
    }

    public static UserDataLoginDto ToDataLoginDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDataLoginDto(user._id,
                                    user.Name,
                                    user.Email,
                                    user.Password,
                                    user.Role);
    }
}
