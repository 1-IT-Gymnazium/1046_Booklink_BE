using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BooklinkBE.Data.Models;
[Table(nameof(Room))]
public class Room
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid HouseholdId { get; set; }
    [JsonIgnore]
    public Household Household { get; set; }
    public ICollection<Bookshelf>? Bookshelves { get; set; }
}
public class CreateRoomRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public Guid HouseholdId { get; set; }
    [Required]
    public Guid UserId { get; set; }
}
public class UpdateRoomRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public Guid HouseholdId { get; set; }
}
