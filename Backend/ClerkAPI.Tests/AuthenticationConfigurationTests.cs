using Clerk.Net.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClerkAPI.Tests;

/// <summary>
/// Configuration can arrive late (test hosts, Aspire, Azure App Configuration), so the JWT
/// settings must come from the final configuration, not from a snapshot taken while the
/// services were being registered.
/// </summary>
public sealed class AuthenticationConfigurationTests(ClerkApiFactory factory) : IClassFixture<ClerkApiFactory>
{
    [Fact]
    public void Jwt_bearer_uses_the_configured_clerk_authority()
    {
        var options = factory.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.Equal("https://example.clerk.accounts.dev", options.Authority);
    }

    [Fact]
    public void Jwt_bearer_keeps_clock_skew_tight_for_short_lived_clerk_tokens()
    {
        var options = factory.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.True(options.TokenValidationParameters.ClockSkew <= TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void Clerk_api_client_uses_the_configured_secret_key()
    {
        var options = factory.Services.GetRequiredService<IOptions<ClerkApiClientOptions>>();

        Assert.Equal("sk_test_not_a_real_key", options.Value.SecretKey);
    }
}
