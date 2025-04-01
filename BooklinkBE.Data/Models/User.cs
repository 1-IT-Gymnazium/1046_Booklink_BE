using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BooklinkBE.Data.Models;
[Table(nameof(User))]
public sealed class User : IdentityUser<Guid>
{
    public ICollection<Book> Books { get; set; }
    public ICollection<Household>? Households { get; set; } = new List<Household>();
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; } = null;
    public DateTime? DeletedAt { get; set; } = null;
}