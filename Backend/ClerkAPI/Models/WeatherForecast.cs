namespace ClerkAPI.Models;

/// <summary>
/// Sample payload returned by <see cref="Controllers.WeatherForecastController"/>.
/// Replace it with your own models.
/// </summary>
public sealed class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
