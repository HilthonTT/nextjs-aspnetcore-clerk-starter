namespace ClerkAPI.Infrastructure;

/// <summary>
/// Source-generated log messages. The generator emits allocation-free, strongly typed helpers,
/// and keeps every message template in one place instead of scattered through the controllers.
/// </summary>
internal static partial class Log
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Error,
        Message = "Unhandled exception while processing {Method} {Path}")]
    public static partial void UnhandledException(ILogger logger, Exception exception, string method, string path);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Token was valid but Clerk has no user {UserId}")]
    public static partial void ClerkUserNotFound(ILogger logger, string userId);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "Weather forecast requested by {UserId}")]
    public static partial void WeatherForecastRequested(ILogger logger, string? userId);
}
