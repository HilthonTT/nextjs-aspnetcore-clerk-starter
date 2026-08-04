namespace ClerkAPI.Options;

/// <summary>
/// CORS settings, bound from the "Cors" configuration section.
/// </summary>
/// <remarks>
/// The template's frontend calls this API from a React Server Component, so the browser is never
/// involved and CORS is not needed. Configure origins here if you also call the API from client
/// components or another browser app.
/// </remarks>
public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    public const string PolicyName = "Frontend";

    public string[] AllowedOrigins { get; set; } = [];
}
