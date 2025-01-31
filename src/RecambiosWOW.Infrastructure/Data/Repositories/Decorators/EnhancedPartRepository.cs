using System.Text;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Interfaces.Providers.Database;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Search;
using RecambiosWOW.Core.Search.SearchEnhancement;
using RecambiosWOW.Infrastructure.Data.Models;

namespace RecambiosWOW.Infrastructure.Data.Repositories.Decorators;

public class EnhancedPartRepository : IPartRepository
{
    private readonly IPartRepository _innerRepository;
    private readonly IDbProvider _dbProvider;
    private readonly ISearchEnhancementService _searchEnhancement;
    private readonly ILogger<EnhancedPartRepository> _logger;

    public EnhancedPartRepository(
        IPartRepository innerRepository,
        IDbProvider dbProvider,
        ISearchEnhancementService searchEnhancement,
        ILogger<EnhancedPartRepository> logger)
    {
        _innerRepository = innerRepository;
        _dbProvider = dbProvider;
        _searchEnhancement = searchEnhancement;
        _logger = logger;
    }

    public Task<Part> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Part> GetBySerialNumberAsync(string serialNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Core.Domain.Entities.Part>> SearchAsync(PartSearchCriteria criteria)
    {
        if (string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            return await _innerRepository.SearchAsync(criteria);
        }

        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            
            // Generate enhanced search terms using AI
            var searchTerms = await _searchEnhancement.GenerateSearchTermsAsync(criteria.SearchTerm);
            
            // Build the search query
            var query = new StringBuilder();
            query.Append("SELECT p.* FROM Parts p ");
            query.Append("JOIN PartsSearch ps ON p.Id = ps.Id ");
            query.Append("WHERE ");

            var searchConditions = searchTerms.Select(term => 
                $"(ps.Content LIKE '%{term}%' OR ps.Keywords LIKE '%{term}%')");
            query.Append(string.Join(" OR ", searchConditions));

            // Apply other criteria filters
            if (criteria.Condition.HasValue)
            {
                query.Append($" AND p.Condition = {(int)criteria.Condition.Value}");
            }

            // Add sorting, pagination etc.
            var parts = await db.QueryAsync<PartModel>(query.ToString());
            
            // Calculate relevance scores and sort results
            var results = new List<(Core.Domain.Entities.Part Part, double Score)>();
            foreach (var part in parts.Select(p => p.ToDomain()))
            {
                var score = await _searchEnhancement.CalculateRelevanceScoreAsync(
                    criteria.SearchTerm,
                    $"{part.Name} {part.Description}");
                results.Add((part, score));
            }

            return results
                .OrderByDescending(r => r.Score)
                .Skip(criteria.Skip)
                .Take(criteria.Take)
                .Select(r => r.Part);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing enhanced search with criteria {@Criteria}", criteria);
            // Fall back to basic search
            return await _innerRepository.SearchAsync(criteria);
        }
    }
    

    // Implement other IPartRepository methods by delegating to _innerRepository
}