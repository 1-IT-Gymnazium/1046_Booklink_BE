using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooklinkBE.Data.Models;
[Table(nameof(RealEstate))]
public class RealEstate
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public ICollection<Book>? Books { get; set; }
    public ICollection<Room>? Rooms { get; set; } = new List<Room>();
}

public class CreateRealEstateRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
}

public class UpdateRealEstateRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}