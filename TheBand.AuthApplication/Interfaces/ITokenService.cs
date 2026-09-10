using TheBand.AuthApplication.Dtos;

namespace TheBand.AuthApplication.Interfaces;

public interface ITokenService
{
    Task<string> GenerateToken(UserDataLoginDto userDataLoginDto);
}
