using Microsoft.OpenApi.Models;
using PmcDashboard.Api.Repositories;
using PmcDashboard.Api.Services;

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
builder.Services.AddScoped<IDashboardRepository, SqlDashboardRepository>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/analytics"))
    {
        await next();
        return;
    }

    var configuredAnalyticsToken = app.Configuration["apiAnalytics:analyticsToken"];
    if (string.IsNullOrWhiteSpace(configuredAnalyticsToken))
    {
        throw new InvalidOperationException(
            "Missing apiAnalytics:analyticsToken. Set it in .env using apiAnalytics__analyticsToken.");
    }

    const string headerName = "X-Analytics-Token";

    if (!context.Request.Headers.TryGetValue(headerName, out var providedAnalyticsToken) ||
        !string.Equals(providedAnalyticsToken.ToString(), configuredAnalyticsToken, StringComparison.Ordinal))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Invalid or missing analytics token.");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();

static void LoadDotEnv()
{
    var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (!File.Exists(envPath))
    {
        return;
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
