namespace TheBand.CoreDomain.Entities;

public class Vinyl
{
    public Guid Guid { get; set; }
    public required string Artist { get; set; }
    public required string Album { get; set; }
    public int Year { get; set; }
    public required string Photo { get; set; }
    public decimal Price { get; set; }
    public bool Active { get; set; }
    public required string UserId { get; set; }
}
