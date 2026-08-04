using System.Security.Claims;

namespace ClerkAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Returns the Clerk user id (the token's "sub" claim, e.g. "user_2ab..."), or null when absent.
    /// </summary>
    public static string? GetUserId(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.NameIdentifier);
}
