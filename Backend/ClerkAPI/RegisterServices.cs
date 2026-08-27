using System.Security.Claims;
using Clerk.Net.DependencyInjection;
using ClerkAPI.Infrastructure;
using ClerkAPI.OpenApi;
using ClerkAPI.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ClerkAPI;

/// <summary>
/// Every service registration for the API, grouped one concern per method.
/// </summary>
public static class RegisterServices
{
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
        builder.Services.AddClerkAuthentication(builder.Configuration);
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

        var clerk = configuration.ReadClerkOptions();

        services.AddClerkApiClient(config => config.SecretKey = clerk.SecretKey);
    }

    /// <summary>
    /// Validates Clerk session tokens as JWT bearer tokens using Clerk's JWKS endpoint.
    /// </summary>
    private static void AddClerkAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var clerk = configuration.ReadClerkOptions();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // The authority is the URL of your Clerk instance; keys are discovered from its JWKS.
                options.Authority = clerk.Authority;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Clerk session tokens carry no audience by default.
                    ValidateAudience = false,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
                options.Events = new JwtBearerEvents
                {
                    // Reject tokens minted for a different frontend origin (the "azp" claim).
                    OnTokenValidated = context =>
                    {
                        var azp = context.Principal?.FindFirstValue("azp");

                        if (string.IsNullOrEmpty(azp) || !azp.Equals(clerk.AuthorizedParty, StringComparison.Ordinal))
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
        var cors = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>() ?? new CorsOptions();

        services.AddCors(options => options.AddPolicy(CorsOptions.PolicyName, policy =>
        {
            if (cors.AllowedOrigins.Length == 0)
            {
                return;
            }

            policy.WithOrigins(cors.AllowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }));
    }

    /// <summary>
    /// Reads the Clerk section eagerly. Values are needed while building the container, before
    /// the options validation registered above would run; that validation still fires at startup.
    /// </summary>
    private static ClerkOptions ReadClerkOptions(this IConfiguration configuration) =>
        configuration.GetSection(ClerkOptions.SectionName).Get<ClerkOptions>() ?? new ClerkOptions();
}
