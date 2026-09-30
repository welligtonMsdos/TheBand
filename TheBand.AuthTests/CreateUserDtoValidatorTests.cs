using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Validators;
using TheBand.AuthDomain.Enum;

namespace TheBand.AuthTests;

public sealed class CreateUserDtoValidatorTests
{
    [Fact]
    public void Validate_ValidCreateUser_ReturnsSuccess()
    {
        var result = new CreateUserDtoValidator().Validate(new CreateUserDto("Maria Silva", "maria@example.com", "SenhaSegura123", UserRole.User));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_InvalidCreateUser_ReturnsErrors()
    {
        var result = new CreateUserDtoValidator().Validate(new CreateUserDto(string.Empty, "email-invalido", "123", (UserRole)99));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserDto.Name));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserDto.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserDto.Password));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserDto.Role));
    }
}
