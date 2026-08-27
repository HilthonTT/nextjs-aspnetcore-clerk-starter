using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ClerkAPI.Tests;

/// <summary>
/// Boots the real API in memory with throwaway Clerk settings, so tests never touch the network
/// and never need a real Clerk account. Tokens are not minted here — the tests below only assert
/// behaviour that does not require one.
/// </summary>
public class ClerkApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["Clerk:SecretKey"] = "sk_test_not_a_real_key",
                ["Clerk:Authority"] = "https://example.clerk.accounts.dev",
                ["Clerk:AuthorizedParty"] = "http://localhost:3000",
                ["Cors:AllowedOrigins:0"] = "http://localhost:3000"
            }));
    }
}
