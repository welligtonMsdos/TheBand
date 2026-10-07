namespace TheBand.CoreApplication.Dtos;

public record ConcertDto(
    Guid Guid,
    string Artist,
    string Venue,
    DateOnly ShowDate,
    string Photo,
    decimal Price);

public record CreateConcertDto(
    string Artist,
    string Venue,
    DateOnly ShowDate,
    string Photo,
    decimal Price);

public record UpdateConcertDto(
    string Artist,
    string Venue,
    DateOnly ShowDate,
    string Photo,
    decimal Price);
