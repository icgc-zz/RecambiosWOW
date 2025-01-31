using System.Text;
using Microsoft.AspNetCore.Mvc;
using RecambiosWOW.Application.Services.Monitoring;
using RecambiosWOW.Core.Interfaces.Monitoring;
using RecambiosWOW.Infrastructure.Monitoring;

namespace RecambiosWOW.Api.Controllers;

[ApiController]
[Route("api/search/metrics")]
public class SearchMetricsController : ControllerBase
{
    private readonly ISearchMetricsCollector _metricsCollector;
    private readonly IAlertService _alertService;

    public SearchMetricsController(
        ISearchMetricsCollector metricsCollector,
        IAlertService alertService)
    {
        _metricsCollector = metricsCollector;
        _alertService = alertService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMetrics([FromQuery] string range = "24h")
    {
        var end = DateTime.UtcNow;
        var start = range switch
        {
            "24h" => end.AddHours(-24),
            "7d" => end.AddDays(-7),
            "30d" => end.AddDays(-30),
            _ => end.AddHours(-24)
        };

        var report = await _metricsCollector.GenerateReportAsync(start, end);
        var alerts = await _alertService.GetActiveAlertsAsync();

        return Ok(new
        {
            report,
            alerts,
            shouldMigrate = report.RecommendElasticsearch,
            migrationReasons = report.RecommendationReasons
        });
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportMetrics([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var report = await _metricsCollector.GenerateReportAsync(start, end);
        
        // Convert to CSV
        var csv = new StringBuilder();
        csv.AppendLine("Timestamp,QueryText,ResultCount,ExecutionTimeMs,ConcurrentSearches,CacheHit");
        
        // Add data rows...

        return File(Encoding.UTF8.GetBytes(csv.ToString()), 
            "text/csv", 
            $"search-metrics-{start:yyyyMMdd}-{end:yyyyMMdd}.csv");
    }
}