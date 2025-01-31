using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.Models;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Interfaces.Services;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Infrastructure.Services.Search;

public class PartSearchService : IPartSearchService
{
    private readonly IPartRepository _partRepository;
    private readonly ICacheService _cacheService;

    public PartSearchService(
        IPartRepository partRepository,
        ICacheService cacheService)
    {
        _partRepository = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public async Task<SearchResult<Part>> SearchAsync(
        PartSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        // Try to get from cache first
        var cacheKey = $"search_{criteria.GetHashCode()}";

        var result = await _cacheService.GetAsync<SearchResult<Part>>(cacheKey);
        if (result != null)
            return result;

        // Perform search
        var parts = await _partRepository.SearchAsync(criteria);
        var totalCount = await _partRepository.GetTotalCountAsync(criteria);

        // Create result
        result = new SearchResult<Part>(
            parts.ToList().AsReadOnly(),
            totalCount,
            criteria.Skip / criteria.Take + 1,
            criteria.Take);

        // Cache the result
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}