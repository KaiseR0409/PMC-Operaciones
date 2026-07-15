using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace PmcDashboard.Api.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        var statusCode = exception switch
        {
            InvalidOperationException => (int)HttpStatusCode.InternalServerError,
            SqlException => (int)HttpStatusCode.ServiceUnavailable,
            TimeoutException => (int)HttpStatusCode.GatewayTimeout,
            OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = GetDetail(exception, httpContext.Request.Path),
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails, JsonOptions),
            cancellationToken);

        return true;
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        400 => "Solicitud inválida",
        503 => "Servicio no disponible",
        504 => "Tiempo de espera agotado",
        _ => "Error interno del servidor"
    };

    private static string GetDetail(Exception exception, PathString path) => exception switch
    {
        SqlException =>
            $"Error al conectar con la base de datos para {path}. Intente nuevamente.",
        TimeoutException =>
            $"La consulta para {path} tardó demasiado. Intente con un período más pequeño.",
        OperationCanceledException =>
            $"La solicitud para {path} fue cancelada.",
        _ =>
            $"Ocurrió un error inesperado procesando {path}. Contacte al administrador."
    };
}
