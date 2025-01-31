using RecambiosWOW.Shared.Services.Implementations;
using RecambiosWOW.Shared.Services.Interfaces;

namespace RecambiosWOW.Web.Services;

public class WebMetricsService : BaseMetricsService, IMetricsService
{
    public WebMetricsService(IHttpClientFactory httpClientFactory, ILogger<WebMetricsService> logger)
        : base(httpClientFactory, logger)
    {
    }

    public async Task<DashboardData> GetDashboardDataAsync(TimeSpan timeRange)
    {
        // Web-specific implementation
        return await GetFromApiAsync<DashboardData>($"api/metrics/dashboard?range={timeRange.TotalHours}");
    }
}