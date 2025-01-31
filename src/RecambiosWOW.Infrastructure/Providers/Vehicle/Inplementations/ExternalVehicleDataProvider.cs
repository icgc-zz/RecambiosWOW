using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Models;
using RecambiosWOW.Core.Interfaces.Services;

namespace RecambiosWOW.Infrastructure.Providers.Vehicle.Inplementations;

public class ExternalVehicleDataProvider : IVehicleDataProvider
{
    private readonly ILogger<ExternalVehicleDataProvider> _logger;
    private readonly ICacheService _cacheService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ExternalVehicleDataProvider(
        ILogger<ExternalVehicleDataProvider> logger,
        ICacheService cacheService,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _logger = logger;
        _cacheService = cacheService;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<VehicleInfo> GetByVinAsync(string vin, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"vehicle_vin_{vin}";
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            try
            {
                // Implementation for external API call
                // This is a placeholder - actual implementation would depend on the specific API being used
                var response = await _httpClient.GetAsync($"api/vehicles/vin/{vin}", cancellationToken);
                response.EnsureSuccessStatusCode();

                var vehicleData = await response.Content.ReadFromJsonAsync<VehicleInfo>(cancellationToken: cancellationToken);
                return vehicleData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vehicle data for VIN: {VIN}", vin);
                throw;
            }
        }, TimeSpan.FromHours(24));
    }

    public async Task<VehicleInfo?> GetByPlateAsync(string plate, string countryCode, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"vehicle_plate_{countryCode}_{plate}";
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            try
            {
                // Implementation for external API call
                // This is a placeholder - actual implementation would depend on the specific API being used
                var response = await _httpClient.GetAsync(
                    $"api/vehicles/plate/{countryCode}/{plate}", 
                    cancellationToken);
                response.EnsureSuccessStatusCode();

                var vehicleData = await response.Content.ReadFromJsonAsync<VehicleInfo>(cancellationToken: cancellationToken);
                return vehicleData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vehicle data for plate: {Plate} ({Country})", plate, countryCode);
                throw;
            }
        }, TimeSpan.FromHours(24));
    }

    public async Task<bool> ValidateVinAsync(string vin, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17)
            return false;

        try
        {
            await GetByVinAsync(vin, cancellationToken);
            return true;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating VIN: {VIN}", vin);
            return false;
        }
    }
} 