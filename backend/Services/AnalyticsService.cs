using System.Globalization;
using PmcDashboard.Api.Dtos;
using PmcDashboard.Api.Models;
using PmcDashboard.Api.Repositories;
using PmcDashboard.Api.Services.Caching;

namespace PmcDashboard.Api.Services;

public sealed class AnalyticsService : IAnalyticsService
{
    private readonly IDashboardCache _cache;
    private readonly ILogger<AnalyticsService> _logger;
    private static readonly CultureInfo ChileCulture = CultureInfo.GetCultureInfo("es-CL");

    public AnalyticsService(IDashboardCache cache, ILogger<AnalyticsService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DespachoRawRow>> GetRawAsync(
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        return await _cache.GetDespachosAsync(year, month, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetClientsAsync(
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var rows = await _cache.GetDespachosAsync(year, month, cancellationToken);

        return rows
            .Select(row => row.Sucursal)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<SummaryDto> GetSummaryAsync(
        int year,
        int month,
        string? client,
        CancellationToken cancellationToken)
    {
        var rows = await _cache.GetDespachosAsync(year, month, cancellationToken);
        var filteredRows = ApplyClientFilter(rows, client).ToList();

        return new SummaryDto(
            TotalSacos: SumByFormat(filteredRows, "Sacos"),
            TotalMaxisacos: SumByFormat(filteredRows, "Maxisacos"),
            TotalGranel: SumByFormat(filteredRows, "Granel"),
            ClientesActivos: filteredRows
                .Select(row => row.Sucursal)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count(),
            RegistrosTotales: filteredRows.Count,
            Variations: new SummaryVariationsDto(0, 0, 0, 0, 0));
    }

    public async Task<IReadOnlyList<ClientTableRowDto>> GetClientTableAsync(
        int year,
        int month,
        string client,
        string? turno,
        CancellationToken cancellationToken)
    {
        var rows = await _cache.GetDespachosAsync(year, month, cancellationToken);

        return rows
            .Where(row => IsSame(row.Sucursal, client))
            .Where(row => string.IsNullOrWhiteSpace(turno) || IsSame(row.Turno, turno))
            .Where(row => row.Fecha is not null)
            .GroupBy(row => new
            {
                Fecha = row.Fecha!.Value.Date,
                row.Turno
            })
            .OrderBy(group => group.Key.Fecha)
            .ThenBy(group => group.Key.Turno)
            .Select(group => new ClientTableRowDto(
                Fecha: group.Key.Fecha.ToString("dd-MM-yyyy", ChileCulture),
                DiaSemana: ChileCulture.TextInfo.ToTitleCase(
                    ChileCulture.DateTimeFormat.GetDayName(group.Key.Fecha.DayOfWeek)),
                Turno: group.Key.Turno,
                Sacos: SumByFormat(group, "Sacos"),
                Maxisacos: SumByFormat(group, "Maxisacos"),
                Granel: SumByFormat(group, "Granel")))
            .ToList();
    }

    public async Task<IReadOnlyList<LineChartPointDto>> GetLineChartAsync(
        int year,
        int month,
        string? client,
        string? product,
        CancellationToken cancellationToken)
    {
        var rows = await _cache.GetDespachosAsync(year, month, cancellationToken);

        return ApplyClientFilter(rows, client)
            .Where(row => string.IsNullOrWhiteSpace(product) || IsSame(row.Formato, product))
            .Where(row => row.Fecha is not null)
            .GroupBy(row => new
            {
                Fecha = row.Fecha!.Value.Date,
                Semana = row.SemanaAno
            })
            .OrderBy(group => group.Key.Fecha)
            .Select(group => new LineChartPointDto(
                Fecha: group.Key.Fecha.ToString("dd-MM-yyyy", ChileCulture),
                Semana: group.Key.Semana,
                Cantidad: group.Sum(row => row.Despacho)))
            .ToList();
    }

    public async Task<IReadOnlyList<TruckChartPointDto>> GetTruckChartAsync(
        int year,
        int month,
        string? client,
        CancellationToken cancellationToken)
    {
        var rows = await _cache.GetDespachosAsync(year, month, cancellationToken);

        return ApplyClientFilter(rows, client)
            .Where(row => row.Fecha is not null)
            .GroupBy(row => new
            {
                Fecha = row.Fecha!.Value.Date,
                Semana = row.SemanaAno
            })
            .OrderBy(group => group.Key.Fecha)
            .Select(group => new TruckChartPointDto(
                Fecha: group.Key.Fecha.ToString("dd-MM-yyyy", ChileCulture),
                Semana: group.Key.Semana,
                Camiones: group.Sum(row => row.Camiones)))
            .ToList();
    }

    private static IEnumerable<DespachoRawRow> ApplyClientFilter(
        IEnumerable<DespachoRawRow> rows,
        string? client)
    {
        return string.IsNullOrWhiteSpace(client)
            ? rows
            : rows.Where(row => IsSame(row.Sucursal, client));
    }

    private static decimal SumByFormat(IEnumerable<DespachoRawRow> rows, string format)
    {
        return rows
            .Where(row => IsSame(row.Formato, format))
            .Sum(row => row.Despacho);
    }

    private static bool IsSame(string value, string expected)
    {
        return string.Equals(value.Trim(), expected.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
