using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using PmcDashboard.Api.Models;
using PmcDashboard.Api.Repositories;

namespace PmcDashboard.Api.Services.Caching;

public sealed class DashboardCache : IDashboardCache
{
    private readonly IDashboardRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DashboardCache> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public DashboardCache(
        IDashboardRepository repository,
        IMemoryCache cache,
        ILogger<DashboardCache> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DespachoRawRow>> GetDespachosAsync(
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"despachos_{year}_{month}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<DespachoRawRow>? cached))
        {
            _logger.LogDebug("Cache HIT for {CacheKey}", cacheKey);
            return cached!;
        }

        _logger.LogInformation("Cache MISS for {CacheKey}. Querying database...", cacheKey);

        var stopwatch = Stopwatch.StartNew();
        var rows = await _repository.GetDespachosAsync(year, month, cancellationToken);
        stopwatch.Stop();

        _logger.LogInformation(
            "Database query completed in {ElapsedMs}ms. Retrieved {Count} rows for {CacheKey}",
            stopwatch.ElapsedMilliseconds,
            rows.Count,
            cacheKey);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(CacheDuration)
            .SetSize(rows.Count);

        _cache.Set(cacheKey, rows, cacheOptions);

        return rows;
    }
}
