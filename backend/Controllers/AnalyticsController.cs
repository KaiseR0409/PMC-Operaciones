using Microsoft.AspNetCore.Mvc;
using PmcDashboard.Api.Dtos;
using PmcDashboard.Api.Services;

namespace PmcDashboard.Api.Controllers;

[ApiController]
[Route("analytics")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    [HttpGet("clients")]
    public async Task<IActionResult> Clients(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var validationError = ValidatePeriod(year, month);
        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        _logger.LogInformation("Fetching clients for {Year}-{Month}", year, month);
        var clients = await _analyticsService.GetClientsAsync(year, month, cancellationToken);
        return Ok(clients);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] string? client,
        CancellationToken cancellationToken)
    {
        var validationError = ValidatePeriod(year, month);
        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        _logger.LogInformation("Fetching summary for {Year}-{Month} (client: {Client})", year, month, client ?? "all");
        var summary = await _analyticsService.GetSummaryAsync(year, month, client, cancellationToken);
        return Ok(summary);
    }

    [HttpGet("client-table")]
    public async Task<IActionResult> ClientTable(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] string client,
        [FromQuery] string? turno,
        CancellationToken cancellationToken)
    {
        var validationError = ValidatePeriod(year, month);
        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        if (string.IsNullOrWhiteSpace(client))
        {
            return BadRequest(new ApiErrorDto("client is required."));
        }

        _logger.LogInformation("Fetching client table for {Year}-{Month} (client: {Client}, turno: {Turno})", year, month, client, turno ?? "all");
        var rows = await _analyticsService.GetClientTableAsync(year, month, client, turno, cancellationToken);
        return Ok(rows);
    }

    [HttpGet("line-chart")]
    public async Task<IActionResult> LineChart(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] string? client,
        [FromQuery] string? product,
        CancellationToken cancellationToken)
    {
        var validationError = ValidatePeriod(year, month);
        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        _logger.LogInformation("Fetching line chart for {Year}-{Month} (client: {Client}, product: {Product})", year, month, client ?? "all", product ?? "all");
        var points = await _analyticsService.GetLineChartAsync(year, month, client, product, cancellationToken);
        return Ok(points);
    }

    [HttpGet("truck-chart")]
    public async Task<IActionResult> TruckChart(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] string? client,
        CancellationToken cancellationToken)
    {
        var validationError = ValidatePeriod(year, month);
        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        _logger.LogInformation("Fetching truck chart for {Year}-{Month} (client: {Client})", year, month, client ?? "all");
        var points = await _analyticsService.GetTruckChartAsync(year, month, client, cancellationToken);
        return Ok(points);
    }

    private static ApiErrorDto? ValidatePeriod(int year, int month)
    {
        if (year <= 0)
        {
            return new ApiErrorDto("year is required.");
        }

        if (month is < 1 or > 12)
        {
            return new ApiErrorDto("month must be between 1 and 12.");
        }

        return null;
    }
}
