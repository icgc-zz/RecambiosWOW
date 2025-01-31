using System.Text;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Exceptions;
using RecambiosWOW.Core.Interfaces.Providers.Database;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite.Repositories;

public class SqlitePartRepository : IPartRepository
{
    private readonly IDbProvider _dbProvider;
    private readonly ILogger<SqlitePartRepository> _logger;

    public SqlitePartRepository(
        IDbProvider dbProvider,
        ILogger<SqlitePartRepository> logger)
    {
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Part> GetByIdAsync(int id)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var part = await connection.GetAsync<Part>(id);
            if (part == null)
                throw new NotFoundException($"Part with ID {id} not found");
            return part;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error getting part with ID {Id}", id);
            throw;
        }
    }

    public Task<Part> GetByIdentifierAsync(string manufacturer, string partNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<Part> GetBySerialNumberAsync(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            throw new ArgumentNullException(nameof(serialNumber));

        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var part = await connection.QueryFirstOrDefaultAsync<Part>(
                "SELECT * FROM Parts WHERE SerialNumber = @SerialNumber",
                new { SerialNumber = serialNumber });

            if (part == null)
                throw new NotFoundException($"Part with serial number {serialNumber} not found");

            return part;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error getting part with serial number {SerialNumber}", serialNumber);
            throw;
        }
    }

    public async Task<IEnumerable<Part>> SearchAsync(PartSearchCriteria criteria)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            
            var (query, parameters) = BuildSearchQuery(criteria, includePaging: true);
            return await connection.QueryAsync<Part>(query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching parts with criteria {@Criteria}", criteria);
            throw;
        }
    }

public async Task<int> GetTotalCountAsync(PartSearchCriteria criteria)
{
    try
    {
        await using var connection = await _dbProvider.GetConnectionAsync();
        
        var query = new StringBuilder("SELECT COUNT(*) FROM Parts WHERE 1=1");
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query.Append(" AND (Name LIKE @SearchTerm OR Description LIKE @SearchTerm OR Manufacturer LIKE @SearchTerm)");
            parameters["SearchTerm"] = $"%{criteria.SearchTerm}%";
        }

        if (criteria.Condition.HasValue)
        {
            query.Append(" AND Condition = @Condition");
            parameters["Condition"] = criteria.Condition.Value;
        }

        if (!string.IsNullOrWhiteSpace(criteria.Manufacturer))
        {
            query.Append(" AND Manufacturer = @Manufacturer");
            parameters["Manufacturer"] = criteria.Manufacturer;
        }

        if (criteria.MinPrice.HasValue)
        {
            query.Append(" AND Price >= @MinPrice");
            parameters["MinPrice"] = criteria.MinPrice.Value;
        }

        if (criteria.MaxPrice.HasValue)
        {
            query.Append(" AND Price <= @MaxPrice");
            parameters["MaxPrice"] = criteria.MaxPrice.Value;
        }

        return await connection.ExecuteScalarAsync<int>(query.ToString(), parameters);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting total count for criteria {@Criteria}", criteria);
        throw;
    }
}

    public async Task<Part> AddAsync(Part part)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var id = await connection.InsertAsync(part);
            return await GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding part {@Part}", part);
            throw;
        }
    }

    public async Task<Part> UpdateAsync(Part part)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var result = await connection.UpdateAsync(part);
            if (result == 0)
                throw new NotFoundException($"Part with ID {part.Id} not found");
            
            return part;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error updating part {@Part}", part);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var result = await connection.DeleteAsync<Part>(id);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting part with ID {Id}", id);
            throw;
        }
    }

    private static (string Query, Dictionary<string, object> Parameters) BuildSearchQuery(
        PartSearchCriteria criteria, 
        bool includePaging)
    {
        var query = new StringBuilder("SELECT * FROM Parts WHERE 1=1");
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query.Append(" AND (Name LIKE @SearchTerm OR Description LIKE @SearchTerm OR Manufacturer LIKE @SearchTerm)");
            parameters["SearchTerm"] = $"%{criteria.SearchTerm}%";
        }

        if (criteria.Condition.HasValue)
        {
            query.Append(" AND Condition = @Condition");
            parameters["Condition"] = criteria.Condition.Value;
        }

        if (!string.IsNullOrWhiteSpace(criteria.Manufacturer))
        {
            query.Append(" AND Manufacturer = @Manufacturer");
            parameters["Manufacturer"] = criteria.Manufacturer;
        }

        if (criteria.MinPrice.HasValue)
        {
            query.Append(" AND Price >= @MinPrice");
            parameters["MinPrice"] = criteria.MinPrice.Value;
        }

        if (criteria.MaxPrice.HasValue)
        {
            query.Append(" AND Price <= @MaxPrice");
            parameters["MaxPrice"] = criteria.MaxPrice.Value;
        }

        // Add sorting
        if (!string.IsNullOrWhiteSpace(criteria.SortBy))
        {
            query.Append($" ORDER BY {criteria.SortBy} {(criteria.SortDescending ? "DESC" : "ASC")}");
        }
        else
        {
            query.Append(" ORDER BY Id");
        }

        if (includePaging)
        {
            // Add pagination
            query.Append(" LIMIT @Take OFFSET @Skip");
            parameters["Take"] = criteria.Take;
            parameters["Skip"] = criteria.Skip;
        }

        return (query.ToString(), parameters);
    }
}