namespace TheBand.CoreApplication.Dtos;

public record CDDto(
    Guid Guid,
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);

public record CreateCDDto(
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);

public record UpdateCDDto(
    string Artist,
    string Album,
    int Year,
    string Photo,
    decimal Price);
