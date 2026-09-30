namespace TheBand.CoreApplication.Dtos;

public record CassetteDto(
    Guid Guid,
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);

public record CreateCassetteDto(
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);

public record UpdateCassetteDto(
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);
