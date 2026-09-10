using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Interfaces;

namespace TheBand.AuthApplication.Services;

public class TokenService : ITokenService
{
    public async Task<string> GenerateToken(UserDataLoginDto userDataLoginDto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var keyVault = Environment.GetEnvironmentVariable("JwtSettings__Key");

        ArgumentNullException.ThrowIfNull(keyVault);

        var key = Encoding.ASCII.GetBytes(keyVault);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
           {
                    new Claim("email", userDataLoginDto.Email),
                    new Claim("id", userDataLoginDto._id),
                    new Claim("name", userDataLoginDto.Name),
                    new Claim("role", userDataLoginDto.Role.ToString())
           }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = "http://localhost:5001",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
