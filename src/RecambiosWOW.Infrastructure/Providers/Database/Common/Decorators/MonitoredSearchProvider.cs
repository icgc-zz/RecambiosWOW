using System.Diagnostics;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.Models;
using RecambiosWOW.Core.Interfaces.Monitoring;
using RecambiosWOW.Core.Interfaces.Search;
using RecambiosWOW.Core.Monitoring;
using RecambiosWOW.Core.Search;
using RecambiosWOW.Infrastructure.Monitoring;

namespace RecambiosWOW.Infrastructure.Providers.Database.Common.Decorators;

public class MonitoredSearchProvider : ISearchProvider
{
    private readonly ISearchProvider _inner;
    private readonly ISearchMetricsCollector _metricsCollector;
    private readonly ILogger<MonitoredSearchProvider> _logger;

    public MonitoredSearchProvider(
        ISearchProvider inner,
        ISearchMetricsCollector metricsCollector,
        ILogger<MonitoredSearchProvider> logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _metricsCollector = metricsCollector ?? throw new ArgumentNullException(nameof(metricsCollector));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SearchResult<Part>> SearchPartsAsync(PartSearchCriteria criteria)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = await _inner.SearchPartsAsync(criteria);
            
            await RecordMetricsAsync(new SearchMetrics
            {
                QueryText = criteria.SearchTerm,
                ResultCount = result.TotalCount,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                PageSize = criteria.Take,
                PageNumber = criteria.Skip / criteria.Take + 1,
                Timestamp = DateTime.UtcNow,
                ConcurrentSearches = 0 // This will be set by the collector
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search with criteria {@Criteria}", criteria);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    public async Task IndexPartAsync(Part part)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _inner.IndexPartAsync(part);
            
            _logger.LogInformation(
                "Indexed part {PartId} in {ElapsedMs}ms", 
                part.Id, 
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing part {PartId}", part.Id);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    public async Task RemoveFromIndexAsync(int partId)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _inner.RemoveFromIndexAsync(partId);
            
            _logger.LogInformation(
                "Removed part {PartId} from index in {ElapsedMs}ms", 
                partId, 
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing part {PartId} from index", partId);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    public async Task RebuildIndexAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _inner.RebuildIndexAsync();
            
            _logger.LogInformation(
                "Rebuilt search index in {ElapsedMs}ms", 
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rebuilding search index");
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private async Task RecordMetricsAsync(SearchMetrics metrics)
    {
        try
        {
            await _metricsCollector.RecordSearchMetricsAsync(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record search metrics");
            // Don't rethrow - we don't want metrics errors to affect the search functionality
        }
    }
}