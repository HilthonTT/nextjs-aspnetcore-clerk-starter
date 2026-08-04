using System.ComponentModel.DataAnnotations;

namespace ClerkAPI.Options;

/// <summary>
/// Clerk settings, bound from the "Clerk" configuration section.
/// All three values are required; the app fails to start without them.
/// </summary>
public sealed class ClerkOptions
{
    public const string SectionName = "Clerk";

    /// <summary>
    /// Backend API key from the Clerk dashboard (starts with "sk_"). Keep it out of source control.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// The URL of your Clerk Frontend API, e.g. "https://verb-noun-00.clerk.accounts.dev".
    /// Used as the JWT authority so tokens are validated against Clerk's JWKS.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [Url]
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Base URL of the frontend that is allowed to call this API, e.g. "http://localhost:3000".
    /// Checked against the "azp" claim of incoming tokens to block token reuse from other origins.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [Url]
    public string AuthorizedParty { get; set; } = string.Empty;
}
