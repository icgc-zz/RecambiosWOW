using System.Text;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Search.SearchEnhancement;

namespace RecambiosWOW.Infrastructure.Services.Search;

public class PartSearchIndexService
{
    private readonly IDatabaseProvider _databaseProvider;
    private readonly ISearchEnhancementService _searchEnhancement;
    private readonly ILogger<PartSearchIndexService> _logger;

    public PartSearchIndexService(
        IDatabaseProvider databaseProvider,
        ISearchEnhancementService searchEnhancement,
        ILogger<PartSearchIndexService> logger)
    {
        _databaseProvider = databaseProvider;
        _searchEnhancement = searchEnhancement;
        _logger = logger;
    }

    public async Task IndexPartAsync(Part part)
    {
        var db = await _databaseProvider.GetConnectionAsync();
        
        // Combine relevant content for searching
        var searchContent = new StringBuilder();
        searchContent.AppendLine(part.Name);
        searchContent.AppendLine(part.Description);
        searchContent.AppendLine(part.Identifier.Manufacturer);
        searchContent.AppendLine(part.Identifier.PartNumber);
        
        // Extract keywords using AI
        var keywords = await _searchEnhancement.ExtractKeywordsAsync(searchContent.ToString());
        
        var searchable = new SearchablePartModel
        {
            Id = part.Id,
            Content = searchContent.ToString(),
            Keywords = string.Join(" ", keywords),
            LastUpdated = DateTime.UtcNow
        };

        await db.RunInTransactionAsync(async (conn) =>
        {
            await conn.InsertOrReplaceAsync(searchable);
        });
    }
}