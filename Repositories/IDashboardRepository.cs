using PmcDashboard.Api.Models;

namespace PmcDashboard.Api.Repositories;

public interface IDashboardRepository
{
    Task<IReadOnlyList<DespachoRawRow>> GetDespachosAsync(int ano, int mes, CancellationToken cancellationToken);
}
