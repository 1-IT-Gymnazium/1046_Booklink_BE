using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BooklinkBE.Data.Models;
[Table(nameof(Household))]
public class Household
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

public class CreateHouseholdRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
}

public class UpdateHouseholdRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}