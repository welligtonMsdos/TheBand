using TheBand.AuthDomain.Enum;

namespace TheBand.AuthApplication.Dtos;

public record UserDto(string _id,
                      string Name,
                      string Email,
                      UserRole Role,
                      bool Active);

public record CreateUserDto(string Name,
                            string Email,
                            string Password,
                            UserRole Role = UserRole.User);

public record UpdateUserDto(string Name,
                            string Email,
                            UserRole Role);

public record UserDataLoginDto(string _id,
                               string Name,
                               string Email,
                               string Token,
                               UserRole Role);

public record UserLoginDto(string Email,
                           string Password);
