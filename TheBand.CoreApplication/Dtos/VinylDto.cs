namespace TheBand.CoreApplication.Dtos;

public record VinylDto(
    Guid Guid, 
    string Artist, 
    string Album, 
    int Year, 
    string Photo, 
    decimal Price);
