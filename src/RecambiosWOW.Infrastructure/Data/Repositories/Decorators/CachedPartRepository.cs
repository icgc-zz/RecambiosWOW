using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Interfaces.Services;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Infrastructure.Data.Repositories.Decorators;

public class CachedPartRepository : IPartRepository
{
    private readonly IPartRepository _inner;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedPartRepository> _logger;
    private const string CacheKeyPrefix = "part";
    private static readonly TimeSpan DefaultCacheTime = TimeSpan.FromMinutes(30);

    public CachedPartRepository(
        IPartRepository inner,
        ICacheService cacheService,
        ILogger<CachedPartRepository> logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Part> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}:id:{id}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _inner.GetByIdAsync(id),
            DefaultCacheTime);
    }

    public async Task<Part> GetBySerialNumberAsync(string serialNumber)
    {
        var cacheKey = $"{CacheKeyPrefix}:serial:{serialNumber}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _inner.GetBySerialNumberAsync(serialNumber),
            DefaultCacheTime);
    }

    public async Task<IEnumerable<Part>> SearchAsync(PartSearchCriteria criteria)
    {
        var cacheKey = $"{CacheKeyPrefix}:search:{criteria.GetHashCode()}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _inner.SearchAsync(criteria),
            TimeSpan.FromMinutes(5));
    }

    public async Task<int> GetTotalCountAsync(PartSearchCriteria criteria)
    {
        var cacheKey = $"{CacheKeyPrefix}:count:{criteria.GetHashCode()}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            () => _inner.GetTotalCountAsync(criteria),
            TimeSpan.FromMinutes(5));
    }

    public async Task<Part> AddAsync(Part part)
    {
        var result = await _inner.AddAsync(part);
        await InvalidatePartCacheAsync(result);
        return result;
    }

    public async Task<Part> UpdateAsync(Part part)
    {
        var result = await _inner.UpdateAsync(part);
        await InvalidatePartCacheAsync(result);
        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _inner.DeleteAsync(id);
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}:id:{id}");
        return result;
    }

    private async Task InvalidatePartCacheAsync(Part part)
    {
        var tasks = new List<Task>
        {
            _cacheService.RemoveAsync($"{CacheKeyPrefix}:id:{part.Id}"),
            _cacheService.RemoveAsync($"{CacheKeyPrefix}:serial:{part.Identifier.SerialNumber}")
        };

        await Task.WhenAll(tasks);
    }
}

