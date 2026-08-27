using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClerkAPI.Tests;

/// <summary>
/// The OpenAPI document is only served in Development, so this fixture overrides the environment.
/// </summary>
public sealed class DevelopmentApiFactory : ClerkApiFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Development");
    }
}

public sealed class OpenApiDocumentTests(DevelopmentApiFactory factory) : IClassFixture<DevelopmentApiFactory>
{
    [Fact]
    public async Task Document_is_served_and_advertises_bearer_auth()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var root = document.RootElement;

        var scheme = root.GetProperty("components").GetProperty("securitySchemes").GetProperty("Bearer");
        Assert.Equal("http", scheme.GetProperty("type").GetString());
        Assert.Equal("bearer", scheme.GetProperty("scheme").GetString());

        var paths = root.GetProperty("paths");
        Assert.True(paths.TryGetProperty("/api/WeatherForecast", out _));
        Assert.True(paths.TryGetProperty("/api/Users/me", out _));
    }
}
