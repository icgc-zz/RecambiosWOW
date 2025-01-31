using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace RecambiosWOW.Shared.Services.Implementations;

public abstract class BaseMetricsService
{
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly ILogger _logger;

    protected BaseMetricsService(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected async Task<T?> GetFromApiAsync<T>(string endpoint)
    {
        using var client = _httpClientFactory.CreateClient("MetricsApi");
        try
        {
            return await client.GetFromJsonAsync<T>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching data from {Endpoint}", endpoint);
            throw;
        }
    }
}
