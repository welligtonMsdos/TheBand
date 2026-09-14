using System.IdentityModel.Tokens.Jwt;
using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Interfaces;
using TheBand.AuthApplication.Services;
using TheBand.AuthDomain.Enum;

namespace TheBand.AuthTests;

public sealed class TokenServiceContractTests
{
    [Fact]
    public async Task ITokenService_GenerateToken_ContainsUserClaims()
    {
        const string key = "a-secure-test-key-with-at-least-32-characters";
        var originalKey = Environment.GetEnvironmentVariable("JwtSettings__Key");
        Environment.SetEnvironmentVariable("JwtSettings__Key", key);

        try
        {
            ITokenService service = new TokenService();
            var token = await service.GenerateToken(new UserDataLoginDto("user-1", "Maria", "maria@example.com", string.Empty, UserRole.Admin));
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Assert.Equal("maria@example.com", jwt.Claims.Single(claim => claim.Type == "email").Value);
            Assert.Equal("user-1", jwt.Claims.Single(claim => claim.Type == "id").Value);
            Assert.Equal(nameof(UserRole.Admin), jwt.Claims.Single(claim => claim.Type == "role").Value);
        }
        finally
        {
            Environment.SetEnvironmentVariable("JwtSettings__Key", originalKey);
        }
    }
}
