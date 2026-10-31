namespace ClerkAPI.Models;

/// <summary>
/// Sample payload returned by <see cref="Controllers.WeatherForecastController"/>.
/// Replace it with your own models.
/// </summary>
public sealed class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)Math.Round(TemperatureC * 9 / 5.0);

    public string? Summary { get; set; }
}
