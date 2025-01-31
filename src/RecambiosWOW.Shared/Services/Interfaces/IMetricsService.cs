namespace RecambiosWOW.Shared.Services.Interfaces;

public interface IMetricsService
{
    Task<DashboardData> GetDashboardDataAsync(TimeSpan timeRange);
    Task<IEnumerable<MetricsData>> GetMetricsAsync(DateTime start, DateTime end);
    Task<IEnumerable<AlertData>> GetAlertsAsync();
}