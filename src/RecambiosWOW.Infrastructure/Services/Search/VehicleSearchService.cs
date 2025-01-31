using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.Models;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Interfaces.Services;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Infrastructure.Services.Search;

public class VehicleSearchService
{
    private readonly IVehicleModelRepository _vehicleModelRepository;
    private readonly ICacheService _cacheService;

    public VehicleSearchService(
        IVehicleModelRepository vehicleModelRepository,
        ICacheService cacheService)
    {
        _vehicleModelRepository = vehicleModelRepository ?? throw new ArgumentNullException(nameof(vehicleModelRepository));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public async Task<SearchResult<Vehicle>> SearchAsync(
        VehicleSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        // Try to get from cache first
        var cacheKey = $"search_{criteria.GetHashCode()}";

        var result = await _cacheService.GetAsync<SearchResult<Vehicle>>(cacheKey);
        if (result != null)
            return result;

        // Perform search
        var vehicles = await _vehicleModelRepository.SearchAsync(criteria);
        var totalCount = await _vehicleModelRepository.GetTotalCountAsync(criteria);

        // Create result
        result = new SearchResult<Vehicle>(
            vehicles.ToList().AsReadOnly(),
            totalCount,
            criteria.Skip / criteria.Take + 1,
            criteria.Take);

        // Cache the result
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}