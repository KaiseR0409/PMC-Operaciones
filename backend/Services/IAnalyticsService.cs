using PmcDashboard.Api.Dtos;
using PmcDashboard.Api.Models;

namespace PmcDashboard.Api.Services;

public interface IAnalyticsService
{
    Task<IReadOnlyList<DespachoRawRow>> GetRawAsync(int year, int month, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> GetClientsAsync(int year, int month, CancellationToken cancellationToken);
    Task<SummaryDto> GetSummaryAsync(int year, int month, string? client, CancellationToken cancellationToken);
    Task<IReadOnlyList<ClientTableRowDto>> GetClientTableAsync(
        int year,
        int month,
        string client,
        string? turno,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<LineChartPointDto>> GetLineChartAsync(
        int year,
        int month,
        string? client,
        string? product,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<TruckChartPointDto>> GetTruckChartAsync(
        int year,
        int month,
        string? client,
        CancellationToken cancellationToken);
}
