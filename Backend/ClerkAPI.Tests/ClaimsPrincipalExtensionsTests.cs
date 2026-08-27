using System.Security.Claims;
using ClerkAPI.Extensions;

namespace ClerkAPI.Tests;

public sealed class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_returns_the_name_identifier_claim()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "user_2abc")], "TestAuth"));

        Assert.Equal("user_2abc", principal.GetUserId());
    }

    [Fact]
    public void GetUserId_returns_null_when_the_claim_is_missing()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        Assert.Null(principal.GetUserId());
    }
}
