using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Validators;

namespace TheBand.CoreTests;

public sealed class VinylDtoValidatorTests
{
    [Fact]
    public void CreateValidator_RequiredFieldsMissing_ReturnsValidationErrors()
    {
        var validator = new CreateVinylDtoValidator();

        var result = validator.Validate(new CreateVinylDto("", "", 999, "", -1));

        Assert.False(result.IsValid);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateVinylDto.Artist));
    }

    [Fact]
    public void UpdateValidator_ValidRequest_PassesValidation()
    {
        var validator = new UpdateVinylDtoValidator();

        var result = validator.Validate(new UpdateVinylDto("Artist", "Album", 2020, "photo.jpg", 10));

        Assert.True(result.IsValid);
    }
}
