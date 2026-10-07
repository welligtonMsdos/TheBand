namespace TheBand.CoreDomain.Entities;

public class Concert
{
    public Guid Guid { get; set; }

    public required string Artist { get; set; }

    public required string Venue { get; set; }

    public DateOnly ShowDate { get; set; }

    public required string Photo { get; set; }

    public bool Active { get; set; }

    public decimal Price { get; set; }

    public required string UserId { get; set; }
}
