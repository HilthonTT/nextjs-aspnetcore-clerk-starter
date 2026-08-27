using ClerkAPI.Extensions;
using ClerkAPI.Infrastructure;
using ClerkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClerkAPI.Controllers;

/// <summary>
/// Sample secured endpoint. Every request must carry a valid Clerk session token.
/// Delete this controller once you have endpoints of your own.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    /// <summary>
    /// Returns five days of made-up weather.
    /// </summary>
    [HttpGet(Name = "GetWeatherForecast")]
    [ProducesResponseType<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IEnumerable<WeatherForecast>> Get()
    {
        var userId = User.GetUserId();
        Log.WeatherForecastRequested(logger, userId);

        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });

        return Ok(forecasts);
    }
}
