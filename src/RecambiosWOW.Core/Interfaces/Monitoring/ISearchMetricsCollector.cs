using RecambiosWOW.Core.Monitoring;

namespace RecambiosWOW.Core.Interfaces.Monitoring;

public interface ISearchMetricsCollector
{
    Task RecordSearchMetricsAsync(SearchMetrics metrics);
    Task<SearchPerformanceReport> GenerateReportAsync(DateTime start, DateTime end);
    Task<bool> ShouldConsiderElasticsearch();
}