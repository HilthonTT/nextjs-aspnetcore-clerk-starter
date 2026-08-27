using ClerkAPI;
using ClerkAPI.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    // Document at /openapi/v1.json, interactive API reference at /scalar.
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("Clerk API"));
}
else
{
    // HTTPS redirection is deliberately NOT applied in development. The Next.js app calls this
    // API server-to-server, and Node's fetch does not trust the ASP.NET Core dev certificate —
    // a redirect to https://localhost:7080 fails with DEPTH_ZERO_SELF_SIGNED_CERT. Locally the
    // traffic never leaves the machine, so plain HTTP is fine. See the README if you would
    // rather run the frontend against HTTPS locally.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors(CorsOptions.PolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Unauthenticated liveness probe, handy for container orchestrators and for checking the API is up.
app.MapHealthChecks("/health").AllowAnonymous();

await app.RunAsync();

/// <summary>
/// Exposed so the test project can boot the real app with <c>WebApplicationFactory&lt;Program&gt;</c>.
/// </summary>
public partial class Program;
