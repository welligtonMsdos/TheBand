namespace TheBand.CoreApplication.Dtos;

public record VinylDto(
    Guid Guid,
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);

public record CreateVinylDto(
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);

public record UpdateVinylDto(
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);
