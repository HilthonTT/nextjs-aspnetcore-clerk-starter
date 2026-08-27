using System.Net;

namespace ClerkAPI.Tests;

/// <summary>
/// Guards the property that matters most in this template: nothing but <c>/health</c> is public.
/// Add a route here whenever you add a controller.
/// </summary>
public sealed class AuthorizationTests(ClerkApiFactory factory) : IClassFixture<ClerkApiFactory>
{
    [Theory]
    [InlineData("/api/WeatherForecast")]
    [InlineData("/api/Users/me")]
    public async Task Secured_endpoints_reject_anonymous_requests(string route)
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(route, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Secured_endpoints_reject_a_garbage_token()
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", "not-a-jwt");

        var response = await client.GetAsync("/api/WeatherForecast", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
