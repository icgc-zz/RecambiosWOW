namespace RecambiosWOW.Infrastructure.Monitoring;

public class SearchPerformanceReport
{
    public double AverageResponseTimeMs { get; set; }
    public double P95ResponseTimeMs { get; set; }
    public double P99ResponseTimeMs { get; set; }
    public int TotalSearches { get; set; }
    public int ConcurrentSearchesMax { get; set; }
    public long IndexSizeBytes { get; set; }
    public int TotalRecords { get; set; }
    public bool RecommendElasticsearch { get; set; }
    public List<string> RecommendationReasons { get; set; } = new();
}