using System.Security.Cryptography;
using System.Text;

namespace PmcDashboard.Api.Middleware;

public sealed class AnalyticsTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AnalyticsTokenMiddleware> _logger;

    public AnalyticsTokenMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<AnalyticsTokenMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/analytics"))
        {
            await _next(context);
            return;
        }

        var configuredToken = _configuration["apiAnalytics:analyticsToken"];
        if (string.IsNullOrWhiteSpace(configuredToken))
        {
            _logger.LogError("Missing apiAnalytics:analyticsToken configuration.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Server configuration error.");
            return;
        }

        const string headerName = "X-Analytics-Token";

        if (!context.Request.Headers.TryGetValue(headerName, out var providedToken))
        {
            LogUnauthorized(context);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid or missing analytics token.");
            return;
        }

        if (!TimingSafeEquals(providedToken.ToString(), configuredToken))
        {
            LogUnauthorized(context);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid or missing analytics token.");
            return;
        }

        await _next(context);
    }

    private void LogUnauthorized(HttpContext context)
    {
        _logger.LogWarning(
            "Unauthorized access attempt to {Path} from {IP}",
            context.Request.Path,
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
    }

    private static bool TimingSafeEquals(string provided, string configured)
    {
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        var configuredBytes = Encoding.UTF8.GetBytes(configured);

        if (providedBytes.Length != configuredBytes.Length)
        {
            CryptographicOperations.ZeroMemory(providedBytes);
            CryptographicOperations.ZeroMemory(configuredBytes);
            return false;
        }

        var result = CryptographicOperations.FixedTimeEquals(providedBytes, configuredBytes);

        CryptographicOperations.ZeroMemory(providedBytes);
        CryptographicOperations.ZeroMemory(configuredBytes);

        return result;
    }
}
