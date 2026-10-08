using TheBand.AuthApplication.Dtos;
using TheBand.AuthApplication.Validators;

namespace TheBand.AuthTests;

public sealed class ChangePasswordDtoValidatorTests
{
    [Fact]
    public void Validate_ValidPasswords_ReturnsSuccess()
    {
        var result = new ChangePasswordDtoValidator().Validate(
            new ChangePasswordDto("SenhaSegura123", "NovaSenha456", "NovaSenha456"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_MissingCurrentPassword_ReturnsError(string? currentPassword)
    {
        var result = new ChangePasswordDtoValidator().Validate(
            new ChangePasswordDto(currentPassword!, "NovaSenha456", "NovaSenha456"));

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangePasswordDto.CurrentPassword));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(7, false)]
    [InlineData(8, true)]
    [InlineData(100, true)]
    [InlineData(101, false)]
    public void Validate_NewPasswordLength_FollowsRegistrationRules(int length, bool expected)
    {
        var password = new string('a', length);

        var result = new ChangePasswordDtoValidator().Validate(
            new ChangePasswordDto("SenhaSegura123", password, password));

        Assert.Equal(expected, result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("OutraSenha789")]
    public void Validate_InvalidConfirmation_ReturnsError(string? confirmation)
    {
        var result = new ChangePasswordDtoValidator().Validate(
            new ChangePasswordDto("SenhaSegura123", "NovaSenha456", confirmation!));

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(ChangePasswordDto.ConfirmNewPassword)
            && error.ErrorMessage == "As senhas não batem.");
    }

    [Fact]
    public void Validate_NullNewPassword_ReturnsError()
    {
        var result = new ChangePasswordDtoValidator().Validate(
            new ChangePasswordDto("SenhaSegura123", null!, "NovaSenha456"));

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangePasswordDto.NewPassword));
    }
}
