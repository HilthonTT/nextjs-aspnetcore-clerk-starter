using Clerk.Net.Client;
using ClerkAPI.Extensions;
using ClerkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClerkAPI.Controllers;

/// <summary>
/// Shows how to go from a validated token to the full Clerk user record via Clerk's Backend API.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly ClerkApiClient _client;

    public UsersController(ILogger<UsersController> logger, ClerkApiClient client)
    {
        _logger = logger;
        _client = client;
    }

    /// <summary>
    /// Returns the profile of the user the request's token belongs to.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType<CurrentUser>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurrentUser>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Fetch this user only — never list every user to find one.
        var user = await _client.Users[userId].GetAsync(cancellationToken: cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Token was valid but Clerk has no user {UserId}", userId);
            return NotFound();
        }

        var primaryEmail = user.EmailAddresses?
            .FirstOrDefault(email => email.Id == user.PrimaryEmailAddressId)?.EmailAddressProp;

        return Ok(new CurrentUser(
            user.Id ?? userId,
            user.FirstName,
            user.LastName,
            primaryEmail,
            user.ImageUrl));
    }
}
