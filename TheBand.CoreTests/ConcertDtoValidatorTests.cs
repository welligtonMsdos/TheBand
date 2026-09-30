using TheBand.CoreApplication.Dtos;
using TheBand.CoreApplication.Validators;

namespace TheBand.CoreTests;

public sealed class ConcertDtoValidatorTests
{
    [Fact]
    public void CreateValidator_RequiredFieldsMissing_ReturnsValidationErrors()
    {
        var validator = new CreateConcertDtoValidator();

        var result = validator.Validate(new CreateConcertDto(string.Empty, string.Empty, default, string.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateConcertDto.Artist));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateConcertDto.Venue));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateConcertDto.ShowDate));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateConcertDto.Photo));
    }

    // [Fact]
    // public void UpdateValidator_ValidRequest_PassesValidation()
    // {
    //     var validator = new UpdateConcertDtoValidator();

    //     var result = validator.Validate(new UpdateConcertDto("Artist", "Venue", new DateOnly(2026, 10, 1), "photo-url.jpg"));

    //     Assert.True(result.IsValid);
    // }

    [Theory]
    [InlineData("", "Venue", "photo-url.jpg", nameof(UpdateConcertDto.Artist))]
    [InlineData("Ar", "Venue", "photo-url.jpg", nameof(UpdateConcertDto.Artist))]
    [InlineData("Artist", "Ve", "photo-url.jpg", nameof(UpdateConcertDto.Venue))]
    [InlineData("Artist", "Venue", "ab", nameof(UpdateConcertDto.Photo))]
    public void UpdateValidator_InvalidTextField_ReturnsValidationError(
        string artist,
        string venue,
        string photo,
        string propertyName)
    {
        var validator = new UpdateConcertDtoValidator();

        var result = validator.Validate(new UpdateConcertDto(artist, venue, new DateOnly(2026, 10, 1), photo));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == propertyName);
    }
}
