using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Validators;

namespace TheBand.CoreTests;

public sealed class CDDtoValidatorTests
{
    [Fact]
    public void CreateValidator_ValidRequest_PassesValidation()
    {
        var result = new CreateCDDtoValidator().Validate(
            new CreateCDDto("Artist", "Album", 2020, "https://photo.jpg", 50));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateValidator_InvalidRequest_ReturnsValidationErrors()
    {
        var result = new UpdateCDDtoValidator().Validate(
            new UpdateCDDto(string.Empty, string.Empty, 1800, "x", 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCDDto.Artist));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCDDto.Album));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCDDto.Year));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCDDto.Photo));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCDDto.Price));
    }
}
