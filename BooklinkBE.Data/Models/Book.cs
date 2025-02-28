using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooklinkBE.Data.Models;
[Table(nameof(Book))]
public class Book
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public string Author { get; set; } = null!;
    [Required]
    public string Genre { get; set; } = null!;
    public string ISBN { get; set; }
    public int PublicationYear { get; set; }
    
    public Guid BookshelfId { get; set; }
    public Bookshelf Bookshelf { get; set; }
}

public class CreateBookRequest
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public int PublicationYear { get; set; }
    
    public Guid BookshelfId { get; set; }
}

public class UpdateBookRequest
{
    [Key]
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public int PublicationYear { get; set; }
    
    [Required]
    public Guid BookshelfId { get; set; }
}