using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooklinkBE.Data.Models;
[Table(nameof(Room))]
public class Room
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid RealEstateId { get; set; }
    
    public RealEstate RealEstate { get; set; }

    public ICollection<Bookshelf>? Bookshelves { get; set; }
}