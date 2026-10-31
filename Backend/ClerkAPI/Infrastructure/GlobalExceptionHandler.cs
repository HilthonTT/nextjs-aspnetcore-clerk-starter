using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClerkAPI.Infrastructure;

/// <summary>
/// Turns any unhandled exception into an RFC 9457 <c>application/problem+json</c> response
/// instead of leaking a stack trace, and logs the original exception.
/// </summary>
internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // The caller went away (closed tab, aborted fetch). Nothing to report and nobody to tell.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            httpContext.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            return true;
        }

        Log.UnhandledException(logger, exception, httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Goes through the same writer as AddProblemDetails, so the response gets the right
        // content type and the traceId/requestId extensions that link it back to the logs.
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            }
        });
    }
}
