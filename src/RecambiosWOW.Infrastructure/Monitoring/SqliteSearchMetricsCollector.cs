using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Monitoring;
using RecambiosWOW.Core.Interfaces.Monitoring;
using RecambiosWOW.Core.Interfaces.Providers.Database;

namespace RecambiosWOW.Infrastructure.Monitoring;

public class SqliteSearchMetricsCollector : ISearchMetricsCollector
{
    private readonly IDbProvider _dbProvider;
    private readonly ILogger<SqliteSearchMetricsCollector> _logger;
    private static readonly SemaphoreSlim _concurrencyCounter = new(1, 1);
    private static int _currentConcurrentSearches;

    public SqliteSearchMetricsCollector(
        IDbProvider dbProvider,
        ILogger<SqliteSearchMetricsCollector> logger)
    {
        _dbProvider = dbProvider;
        _logger = logger;
    }

    public async Task RecordSearchMetricsAsync(SearchMetrics metrics)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            await db.ExecuteAsync(@"
                INSERT INTO SearchMetrics (
                    QueryText,
                    ResultCount,
                    ExecutionTimeMs,
                    PageSize,
                    PageNumber,
                    Timestamp,
                    CacheHit,
                    IndexSizeBytes,
                    ConcurrentSearches
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
                metrics.QueryText,
                metrics.ResultCount,
                metrics.ExecutionTimeMs,
                metrics.PageSize,
                metrics.PageNumber,
                metrics.Timestamp,
                metrics.CacheHit,
                metrics.IndexSizeBytes,
                metrics.ConcurrentSearches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording search metrics");
        }
    }

    public async Task<SearchPerformanceReport> GenerateReportAsync(DateTime start, DateTime end)
    {
        var db = await _dbProvider.GetConnectionAsync();
        
        var metrics = await db.QueryAsync<SearchMetrics>(@"
            SELECT * FROM SearchMetrics 
            WHERE Timestamp BETWEEN ? AND ?",
            start, end);

        var report = new SearchPerformanceReport
        {
            TotalSearches = metrics.Count(),
            AverageResponseTimeMs = metrics.Average(m => m.ExecutionTimeMs),
            P95ResponseTimeMs = CalculatePercentile(metrics.Select(m => m.ExecutionTimeMs), 95),
            P99ResponseTimeMs = CalculatePercentile(metrics.Select(m => m.ExecutionTimeMs), 99),
            ConcurrentSearchesMax = metrics.Max(m => m.ConcurrentSearches),
            IndexSizeBytes = await GetIndexSizeAsync(),
            TotalRecords = await GetTotalRecordsAsync()
        };

        await EvaluateElasticsearchRecommendation(report);
        return report;
    }

    public async Task<bool> ShouldConsiderElasticsearch()
    {
        var report = await GenerateReportAsync(
            DateTime.UtcNow.AddDays(-7), 
            DateTime.UtcNow);

        return report.RecommendElasticsearch;
    }

    private async Task EvaluateElasticsearchRecommendation(SearchPerformanceReport report)
    {
        var reasons = new List<string>();

        // Check response times
        if (report.P95ResponseTimeMs > 500)
        {
            reasons.Add($"95th percentile response time ({report.P95ResponseTimeMs:N0}ms) exceeds 500ms threshold");
        }

        // Check concurrent searches
        if (report.ConcurrentSearchesMax > 50)
        {
            reasons.Add($"Max concurrent searches ({report.ConcurrentSearchesMax}) exceeds 50 threshold");
        }

        // Check index size
        if (report.IndexSizeBytes > 1024L * 1024L * 1024L) // 1GB
        {
            reasons.Add($"Index size ({report.IndexSizeBytes / (1024 * 1024):N0}MB) exceeds 1GB threshold");
        }

        // Check total records
        if (report.TotalRecords > 100000)
        {
            reasons.Add($"Total records ({report.TotalRecords:N0}) exceeds 100,000 threshold");
        }

        report.RecommendElasticsearch = reasons.Any();
        report.RecommendationReasons = reasons;
    }

    private static double CalculatePercentile(IEnumerable<long> sequence, int percentile)
    {
        var sorted = sequence.OrderBy(x => x).ToList();
        var index = (int)Math.Ceiling((percentile / 100.0) * sorted.Count) - 1;
        return sorted[index];
    }

    private async Task<long> GetIndexSizeAsync()
    {
        var db = await _dbProvider.GetConnectionAsync();
        return await db.ExecuteScalarAsync<long>("SELECT page_count * page_size FROM pragma_page_count(), pragma_page_size();");
    }

    private async Task<int> GetTotalRecordsAsync()
    {
        var db = await _dbProvider.GetConnectionAsync();
        return await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Parts");
    }

    public static IDisposable TrackConcurrentSearch()
    {
        Interlocked.Increment(ref _currentConcurrentSearches);
        return new ConcurrentSearchTracker();
    }

    private class ConcurrentSearchTracker : IDisposable
    {
        public void Dispose()
        {
            Interlocked.Decrement(ref _currentConcurrentSearches);
        }
    }
}