using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooklinkBE.Data.Models;
[Table(nameof(Bookshelf))]
public class Bookshelf
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int NumberOfColumns { get; set; }
    public int NumberOfRows { get; set; }
    public Guid RoomId { get; set; }
    public Room Room { get; set; }
    
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
public class CreateBookshelfModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int NumberOfColumns { get; set; }
    public int NumberOfRows { get; set; }

    public Guid RoomId { get; set; }
}
public class UpdateBookshelfModel
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int NumberOfColumns { get; set; }
    public int NumberOfRows { get; set; }
    public Guid RoomId { get; set; }
}