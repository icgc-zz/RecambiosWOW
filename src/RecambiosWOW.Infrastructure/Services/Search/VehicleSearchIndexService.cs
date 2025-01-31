using System.Text;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Interfaces.Providers.Database;
using RecambiosWOW.Core.Search.SearchEnhancement;

namespace RecambiosWOW.Infrastructure.Services.Search;

public class VehicleSearchIndexService
{
    private readonly IDbProvider _dbProvider;
    private readonly ISearchEnhancementService _searchEnhancement;
    private readonly ILogger<VehicleSearchIndexService> _logger;

    public VehicleSearchIndexService(
        IDbProvider dbProvider,
        ISearchEnhancementService searchEnhancement,
        ILogger<VehicleSearchIndexService> logger)
    {
        _dbProvider = dbProvider;
        _searchEnhancement = searchEnhancement;
        _logger = logger;
    }

    public async Task IndexVehicleAsync(Vehicle vehicle)
    {
        var db = await _dbProvider.GetConnectionAsync();
        
        // Combine relevant content for searching
        var searchContent = new StringBuilder();
        searchContent.AppendLine(vehicle.Id.ToString());
        searchContent.AppendLine(vehicle.Identifier.Make);
        searchContent.AppendLine(vehicle.Identifier.Model);
        searchContent.AppendLine(vehicle.Identifier.Year.ToString());
        searchContent.AppendLine(vehicle.LicensePlate);
        
        // Extract keywords using AI
        var keywords = await _searchEnhancement.ExtractKeywordsAsync(searchContent.ToString());
        
        var searchable = new SearchableVehicleModel
        {
            Id = vehicle.Id,
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