using System.ComponentModel.DataAnnotations;

namespace BooklinkBE.API.Entities;

public class Register
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}