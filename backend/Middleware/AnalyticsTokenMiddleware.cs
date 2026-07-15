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

        if (!context.Request.Headers.TryGetValue(headerName, out var providedToken) ||
            !string.Equals(providedToken.ToString(), configuredToken, StringComparison.Ordinal))
        {
            _logger.LogWarning(
                "Unauthorized access attempt to {Path} from {IP}",
                context.Request.Path,
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown");

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid or missing analytics token.");
            return;
        }

        await _next(context);
    }
}
