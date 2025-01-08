namespace Booklink.Models;

public class PhysicalLocation
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    
    public string Name { get; set; } // e.g., "Living Room Bookshelf" or "Living room A3"
    
    public string Description { get; set; } // "The tallest bookshelf next to the window
    
    public string Room { get; set; } // for example: living room
    
    public string Address { get; set; } // For users with multiple properties => city apartment, 

    // Navigation properties
    public virtual User User { get; set; }
    public virtual ICollection<Book> Books { get; set; }
}