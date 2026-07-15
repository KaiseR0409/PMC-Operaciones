using PmcDashboard.Api.Models;

namespace PmcDashboard.Api.Services.Caching;

public interface IDashboardCache
{
    Task<IReadOnlyList<DespachoRawRow>> GetDespachosAsync(
        int year,
        int month,
        CancellationToken cancellationToken);
}
