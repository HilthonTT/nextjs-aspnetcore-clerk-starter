using System.Security.Claims;
using Clerk.Net.DependencyInjection;
using ClerkAPI.Infrastructure;
using ClerkAPI.OpenApi;
using ClerkAPI.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace ClerkAPI;

/// <summary>
/// Every service registration for the API, grouped one concern per method.
/// </summary>
public static class RegisterServices
{
    /// <summary>
    /// Clerk session tokens live for 60 seconds. The JWT handler's default 5-minute clock skew
    /// would keep a token usable six times longer than Clerk intends; this matches Clerk's own SDKs.
    /// </summary>
    private static readonly TimeSpan ClerkClockSkew = TimeSpan.FromSeconds(5);

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddHealthChecks();

        // RFC 9457 problem details for framework-generated failures (401, 404, ...).
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // Built-in OpenAPI document generation (no Swashbuckle needed since .NET 9).
        builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

        builder.Services.AddClerkOptions(builder.Configuration);
        builder.Services.AddClerkAuthentication();
        builder.Services.AddFrontendCors(builder.Configuration);
    }

    /// <summary>
    /// Binds and validates the "Clerk" section, then registers Clerk's Backend API client.
    /// Validation runs at startup, so a missing key fails fast instead of surfacing as a 401 later.
    /// </summary>
    private static void AddClerkOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ClerkOptions>()
            .Bind(configuration.GetSection(ClerkOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddClerkApiClient(_ => { });

        // Resolved from the final configuration rather than read while services are being
        // registered, so values added later (test hosts, secret stores) are honoured.
        services.AddOptions<ClerkApiClientOptions>()
            .Configure<IOptions<ClerkOptions>>((client, clerk) => client.SecretKey = clerk.Value.SecretKey);
    }

    /// <summary>
    /// Validates Clerk session tokens as JWT bearer tokens using Clerk's JWKS endpoint.
    /// </summary>
    private static void AddClerkAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<ClerkOptions>>((options, clerkOptions) =>
            {
                var clerk = clerkOptions.Value;
                var authorizedParty = clerk.AuthorizedParty.TrimEnd('/');

                // The authority is the URL of your Clerk instance; keys are discovered from its JWKS.
                options.Authority = clerk.Authority;

                // Clerk session tokens carry no audience by default.
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = ClaimTypes.NameIdentifier;
                options.TokenValidationParameters.ClockSkew = ClerkClockSkew;

                options.Events = new JwtBearerEvents
                {
                    // Reject tokens minted for a different frontend origin (the "azp" claim).
                    OnTokenValidated = context =>
                    {
                        var azp = context.Principal?.FindFirstValue("azp")?.TrimEnd('/');

                        if (string.IsNullOrEmpty(azp) || !azp.Equals(authorizedParty, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Fail("AZP claim is invalid or missing.");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
    }

    /// <summary>
    /// CORS for browsers that call this API directly. Not used by the template's server-side calls.
    /// </summary>
    private static void AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CorsOptions>().Bind(configuration.GetSection(CorsOptions.SectionName));

        services.AddCors();
        services.AddOptions<Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions>()
            .Configure<IOptions<CorsOptions>>((options, corsOptions) =>
                options.AddPolicy(CorsOptions.PolicyName, policy =>
                {
                    var origins = corsOptions.Value.AllowedOrigins;

                    if (origins.Length == 0)
                    {
                        return;
                    }

                    policy.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }));
    }
}
