using TheBand.AuthDomain.Enum;

namespace TheBand.AuthDomain.Entities;

public class User
{   
    public string _id { get; set; } = string.Empty;
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }   
    public required UserRole Role { get; set; } = UserRole.User;
    public DateTime LastAccess { get; set; }
    public bool Active { get; set; }
}
