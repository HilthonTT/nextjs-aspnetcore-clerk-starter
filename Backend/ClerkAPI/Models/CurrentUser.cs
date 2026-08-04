namespace ClerkAPI.Models;

/// <summary>
/// A trimmed-down projection of the Clerk user, so the API decides what the frontend sees
/// instead of forwarding Clerk's full user object.
/// </summary>
public sealed record CurrentUser(
    string Id,
    string? FirstName,
    string? LastName,
    string? EmailAddress,
    string? ImageUrl);
