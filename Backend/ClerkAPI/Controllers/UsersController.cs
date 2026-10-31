using Clerk.Net.Client;
using ClerkAPI.Extensions;
using ClerkAPI.Infrastructure;
using ClerkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Kiota.Abstractions;

namespace ClerkAPI.Controllers;

/// <summary>
/// Shows how to go from a validated token to the full Clerk user record via Clerk's Backend API.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class UsersController(ILogger<UsersController> logger, ClerkApiClient client) : ControllerBase
{
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

        Clerk.Net.Client.Models.User? user;

        try
        {
            // Fetch this user only — never list every user to find one.
            user = await client.Users[userId].GetAsync(cancellationToken: cancellationToken);
        }
        catch (ApiException exception) when (exception.ResponseStatusCode == StatusCodes.Status404NotFound)
        {
            // The generated Clerk client throws on error responses rather than returning null,
            // e.g. when the user was deleted after the token was issued.
            user = null;
        }

        if (user is null)
        {
            Log.ClerkUserNotFound(logger, userId);
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "The user behind this token no longer exists in Clerk.");
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
