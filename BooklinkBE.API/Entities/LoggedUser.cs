namespace BooklinkBE.API.Entities;

public class LoggedUser
{
    public Guid Id { get; set; }

    public bool IsAdmin { get; set; }

    public string? Name { get; set; } = string.Empty;

    public bool IsAuthenticated { get; set; }
}