namespace TheBand.CoreApplication.Dtos;

public record CreateVinylDto(
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);
