using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Interfaces.Services;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Infrastructure.Data.Repositories.Decorators;

// Infrastructure/Repositories/Decorators/CachedVehicleModelRepository.cs
public class CachedVehicleModelRepository : IVehicleModelRepository
{
    private readonly IVehicleModelRepository _inner;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedVehicleModelRepository> _logger;
    private const string CacheKeyPrefix = "vehicle";
    private static readonly TimeSpan DefaultCacheTime = TimeSpan.FromMinutes(30);

    public CachedVehicleModelRepository(
        IVehicleModelRepository inner,
        ICacheService cacheService,
        ILogger<CachedVehicleModelRepository> logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Vehicle> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}:id:{id}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _inner.GetByIdAsync(id),
            DefaultCacheTime);
    }

    public async Task<Vehicle> GetBySerialNumberAsync(string serialNumber)
    {
        var cacheKey = $"{CacheKeyPrefix}:serial:{serialNumber}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _inner.GetBySerialNumberAsync(serialNumber),
            DefaultCacheTime);
    }

    public async Task<IEnumerable<Vehicle>> SearchAsync(VehicleSearchCriteria criteria)
    {
        var cacheKey = $"{CacheKeyPrefix}:search:{criteria.GetHashCode()}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _inner.SearchAsync(criteria),
            TimeSpan.FromMinutes(5));
    }

    public async Task<int> GetTotalCountAsync(VehicleSearchCriteria criteria)
    {
        var cacheKey = $"{CacheKeyPrefix}:count:{criteria.GetHashCode()}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _inner.GetTotalCountAsync(criteria),
            TimeSpan.FromMinutes(5));
    }

    public async Task<Vehicle> AddAsync(Vehicle vehicle)
    {
        var result = await _inner.AddAsync(vehicle);
        await InvalidateVehicleCacheAsync(result);
        return result;
    }

    public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
    {
        var result = await _inner.UpdateAsync(vehicle);
        await InvalidateVehicleCacheAsync(result);
        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _inner.DeleteAsync(id);
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}:id:{id}");
        return result;
    }

    private async Task InvalidateVehicleCacheAsync(Vehicle vehicle)
    {
        var tasks = new List<Task>
        {
            _cacheService.RemoveAsync($"{CacheKeyPrefix}:id:{vehicle.Id}"),
            _cacheService.RemoveAsync($"{CacheKeyPrefix}:serial:{vehicle.Identifier.SerialNumber}")
        };

        await Task.WhenAll(tasks);
    }
}