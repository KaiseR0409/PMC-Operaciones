using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using PmcDashboard.Api.Middleware;
using PmcDashboard.Api.Repositories;
using PmcDashboard.Api.Services;
using PmcDashboard.Api.Services.Caching;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-Analytics-Token",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Token requerido para consumir endpoints /analytics."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            []
        }
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IDashboardRepository, SqlDashboardRepository>();
builder.Services.AddScoped<IDashboardCache, DashboardCache>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:5173"];

        policy
            .WithOrigins(origins)
            .WithHeaders("X-Analytics-Token")
            .WithMethods("GET");
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AnalyticsLimiter", context =>
    {
        var clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?.Split(',').First().Trim()
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            type = "https://httpstatuses.com/429",
            title = "Demasiadas solicitudes",
            status = 429,
            detail = "Ha excedido el limite de solicitudes. Intente nuevamente en un minuto."
        }, cancellationToken);
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseExceptionHandler();

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseCors("Frontend");

app.UseRateLimiter();

app.UseMiddleware<AnalyticsTokenMiddleware>();

app.UseAuthorization();

app.MapControllers().RequireRateLimiting("AnalyticsLimiter");

app.Run();

static void LoadDotEnv()
{
    var candidatePaths = new[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), ".env"),
        Path.Combine(Directory.GetCurrentDirectory(), "..", ".env")
    }
    .Select(Path.GetFullPath)
    .Distinct(StringComparer.OrdinalIgnoreCase);

    foreach (var envPath in candidatePaths)
    {
        if (!File.Exists(envPath))
        {
            continue;
        }

        foreach (var line in File.ReadAllLines(envPath))
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = trimmedLine.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = trimmedLine[..separatorIndex].Trim();
            var value = trimmedLine[(separatorIndex + 1)..].Trim().Trim('"');

            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
