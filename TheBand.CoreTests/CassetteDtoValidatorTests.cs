using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Validators;

namespace TheBand.CoreTests;

public sealed class CassetteDtoValidatorTests
{
    [Fact]
    public void CreateValidator_ValidRequest_PassesValidation()
    {
        var result = new CreateCassetteDtoValidator().Validate(
            new CreateCassetteDto("Artist", "Album", 2020, "https://photo.jpg", 50));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateValidator_InvalidRequest_ReturnsValidationErrors()
    {
        var result = new UpdateCassetteDtoValidator().Validate(
            new UpdateCassetteDto(string.Empty, string.Empty, 1800, "x", 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCassetteDto.Artist));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCassetteDto.Album));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCassetteDto.Year));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCassetteDto.Photo));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateCassetteDto.Price));
    }
}
