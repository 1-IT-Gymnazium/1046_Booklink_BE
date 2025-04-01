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
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Genre { get; set; } = null!;

    [MaxLength(17)]
    public string? Isbn { get; set; }
    public int PublicationYear { get; set; }

    public bool IsInReadingList { get; set; }
    public int ColumnsFromLeft { get; set; }
    public int RowsFromTop { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid RoomId { get; set; }
    [Required]
    public Guid BookshelfId { get; set; }

    [ForeignKey(nameof(BookshelfId))]
    public Bookshelf Bookshelf { get; set; } = null!;
}

public class CreateBookRequest
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string? Isbn { get; set; }
    public int PublicationYear { get; set; }
    public bool IsInReadingList { get; set; }
    public int ColumnsFromLeft { get; set; }
    public int RowsFromTop { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid RoomId { get; set; }
    public Guid BookshelfId { get; set; }
}

public class UpdateBookRequest
{
    [Key]
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string? Isbn { get; set; }
    public int PublicationYear { get; set; }
    public bool IsInReadingList { get; set; }
    public int ColumnsFromLeft { get; set; }
    public int RowsFromTop { get; set; }

    public Guid HouseholdId { get; set; }
    public Guid RoomId { get; set; }
    public Guid BookshelfId { get; set; }
}