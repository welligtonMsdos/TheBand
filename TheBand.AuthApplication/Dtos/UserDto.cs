using TheBand.AuthDomain.Enum;

namespace TheBand.AuthApplication.Dtos;

public record UserDto(string _id,
                      string Name,
                      string Email);

public record UserDataLoginDto(string _id,
                               string Name,
                               string Email,
                               string Token,
                               UserRole Role);

public record UserLoginDto(string Email,
                           string Password);
