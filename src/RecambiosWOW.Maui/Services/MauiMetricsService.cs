using Microsoft.Extensions.Logging;
using RecambiosWOW.Shared.Services.Implementations;
using RecambiosWOW.Shared.Services.Interfaces;

namespace RecambiosWOW.Maui.Services;

public class MauiMetricsService : BaseMetricsService, IMetricsService
{
    private readonly IConnectivity _connectivity;
    private readonly IPreferences _preferences;

    public MauiMetricsService(
        IHttpClientFactory httpClientFactory,
        ILogger<MauiMetricsService> logger,
        IConnectivity connectivity,
        IPreferences preferences)
        : base(httpClientFactory, logger)
    {
        _connectivity = connectivity;
        _preferences = preferences;
    }

    public async Task<DashboardData> GetDashboardDataAsync(TimeSpan timeRange)
    {
        // Check connectivity
        if (_connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            // Return cached data
            return await GetCachedDataAsync();
        }

        // Get fresh data
        var data = await GetFromApiAsync<DashboardData>($"api/metrics/dashboard?range={timeRange.TotalHours}");
        
        // Cache the data
        await CacheDataAsync(data);
        
        return data;
    }

    private async Task<DashboardData> GetCachedDataAsync()
    {
        // Implementation of offline data access
    }

    private async Task CacheDataAsync(DashboardData data)
    {
        // Implementation of data caching
    }
}