using System.ComponentModel.DataAnnotations;

namespace BooklinkBE.API.Entities;

public class Token
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string SessionToken { get; set; } = null!;
}