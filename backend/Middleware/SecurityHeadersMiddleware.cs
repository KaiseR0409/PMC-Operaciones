namespace PmcDashboard.Api.Middleware;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var response = context.Response;

        response.Headers["X-Content-Type-Options"] = "nosniff";
        response.Headers["X-Frame-Options"] = "DENY";
        response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        response.Headers["X-XSS-Protection"] = "1; mode=block";

        if (!context.Request.IsHttps && context.Request.Host.Port != null)
        {
            response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
        }

        if (context.Request.Path.StartsWithSegments("/analytics"))
        {
            response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            response.Headers["Pragma"] = "no-cache";
        }

        await _next(context);
    }
}
