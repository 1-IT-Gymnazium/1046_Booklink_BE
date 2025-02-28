using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooklinkBE.Data.Models;

[Table(nameof(EmailMessage))]
public class EmailMessage
{
    [Key]
    public Guid Id { get; set; }
    public required string RecipientEmail { get; set; }
    public string? RecipientName { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public bool Sent { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public required string FromEmail { get; set; }
    public required string FromName { get; set; }
}