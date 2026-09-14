using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Validators;
using TheBand.AuthDomain.Enum;

namespace TheBand.AuthTests;

public sealed class UserDtoValidatorTests
{
    [Fact]
    public void Validate_ValidUser_ReturnsSuccess()
    {
        var result = new UserValidator().Validate(new UserDto("user-1", "Maria Silva", "maria.silva@example.com", UserRole.User, true));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_MissingRequiredFields_ReturnsErrors()
    {
        var result = new UserValidator().Validate(new UserDto("user-1", string.Empty, string.Empty, UserRole.User, true));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserDto.Name));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserDto.Email));
    }
}
