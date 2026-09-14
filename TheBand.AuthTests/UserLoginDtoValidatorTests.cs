using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Validators;

namespace TheBand.AuthTests;

public sealed class UserLoginDtoValidatorTests
{
    [Fact]
    public void Validate_ValidLogin_ReturnsSuccess()
    {
        var result = new UserLoginDtoValidator().Validate(new UserLoginDto("maria.silva@example.com", "SenhaSegura123"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "SenhaSegura123")]
    [InlineData("email-invalido", "SenhaSegura123")]
    [InlineData("maria.silva@example.com", "curta")]
    public void Validate_InvalidLogin_ReturnsErrors(string email, string password)
    {
        var result = new UserLoginDtoValidator().Validate(new UserLoginDto(email, password));

        Assert.False(result.IsValid);
    }
}
