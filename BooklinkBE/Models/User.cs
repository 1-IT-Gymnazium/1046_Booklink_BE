namespace Booklink.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public virtual ICollection<Book> Books { get; set; }
    public virtual ICollection<PhysicalLocation> Locations { get; set; }
}
