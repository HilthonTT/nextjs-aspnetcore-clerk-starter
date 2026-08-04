using System.Security.Claims;
using Clerk.Net.DependencyInjection;
using ClerkAPI.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ClerkAPI;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwagger();

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

        var clerk = configuration.GetSection(ClerkOptions.SectionName).Get<ClerkOptions>() ?? new ClerkOptions();

        services.AddClerkApiClient(config => config.SecretKey = clerk.SecretKey);
    }

    /// <summary>
    /// Validates Clerk session tokens as JWT bearer tokens using Clerk's JWKS endpoint.
    /// </summary>
    private static void AddClerkAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var clerk = configuration.GetSection(ClerkOptions.SectionName).Get<ClerkOptions>() ?? new ClerkOptions();

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
    /// Adds Swagger with an "Authorize" button so you can paste a Clerk token and call secured endpoints.
    /// </summary>
    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Clerk API",
                Version = "v1",
                Description = "ASP.NET Core Web API secured with Clerk session tokens."
            });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste a Clerk session token (without the \"Bearer \" prefix).",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = [] });
        });
    }
}
