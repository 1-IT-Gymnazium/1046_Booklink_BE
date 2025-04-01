using System.Security.Claims;

namespace BooklinkBE.API.Utils;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new InvalidOperationException("user not logged in");
        }
        var idString = user.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        return Guid.Parse(idString);
    }
}