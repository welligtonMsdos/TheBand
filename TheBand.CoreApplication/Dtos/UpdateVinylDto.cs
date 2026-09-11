namespace TheBand.CoreApplication.Dtos;

public record UpdateVinylDto(
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);
