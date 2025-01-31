namespace RecambiosWOW.Core.Monitoring;

public class SearchMetrics
{
    public string QueryText { get; set; }
    public int ResultCount { get; set; }
    public long ExecutionTimeMs { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public DateTime Timestamp { get; set; }
    public bool CacheHit { get; set; }
    public long IndexSizeBytes { get; set; }
    public int ConcurrentSearches { get; set; }
}