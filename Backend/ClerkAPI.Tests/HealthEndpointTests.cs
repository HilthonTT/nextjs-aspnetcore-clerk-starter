using System.Net;

namespace ClerkAPI.Tests;

public sealed class HealthEndpointTests(ClerkApiFactory factory) : IClassFixture<ClerkApiFactory>
{
    [Fact]
    public async Task Health_is_reachable_without_a_token()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
