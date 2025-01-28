using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Exceptions;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Interfaces.Database;
using RecambiosWOW.Core.Search;
using RecambiosWOW.Infrastructure.Data.Models;

namespace RecambiosWOW.Infrastructure.Data.Repositories;

public class SqlitePartRepository : IPartRepository, IBatchRepository<Part>
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
            var db = await _dbProvider.GetConnectionAsync();
            var partModel = await db.Table<PartModel>()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (partModel == null)
            {
                _logger.LogWarning("Part with ID {Id} not found", id);
                throw new NotFoundException($"Part with ID {id} not found");
            }

            return partModel.ToDomain();
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error retrieving part with ID {Id}", id);
            throw new RepositoryException($"Error retrieving part with ID {id}", ex);
        }
    }

    public async Task<Part> GetBySerialNumberAsync(string serialNumber)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var partModel = await db.Table<PartModel>()
                .FirstOrDefaultAsync(p => p.SerialNumber == serialNumber);

            if (partModel == null)
            {
                _logger.LogWarning("Part with serial number {SerialNumber} not found", serialNumber);
                throw new NotFoundException($"Part with serial number {serialNumber} not found");
            }

            return partModel.ToDomain();
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error retrieving part with serial number {SerialNumber}", serialNumber);
            throw new RepositoryException($"Error retrieving part with serial number {serialNumber}", ex);
        }
    }

    public async Task<IEnumerable<Part>> SearchAsync(PartSearchCriteria criteria)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var query = db.Table<PartModel>();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                var searchTerm = criteria.SearchTerm.ToLowerInvariant();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm) ||
                    p.Manufacturer.ToLower().Contains(searchTerm));
            }

            if (criteria.Condition.HasValue)
            {
                query = query.Where(p => p.Condition == criteria.Condition.Value);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Manufacturer))
            {
                query = query.Where(p => p.Manufacturer == criteria.Manufacturer);
            }

            if (criteria.MinPrice.HasValue)
            {
                query = query.Where(p => p.PriceAmount >= criteria.MinPrice.Value);
            }

            if (criteria.MaxPrice.HasValue)
            {
                query = query.Where(p => p.PriceAmount <= criteria.MaxPrice.Value);
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(criteria.SortBy))
            {
                query = ApplySorting(query, criteria.SortBy, criteria.SortDescending);
            }

            // Apply pagination
            var results = await query
                .Skip(criteria.Skip)
                .Take(criteria.Take)
                .ToListAsync();

            return results.Select(r => r.ToDomain());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching parts with criteria {@Criteria}", criteria);
            throw new RepositoryException("Error searching parts", ex);
        }
    }

    public async Task<int> GetTotalCountAsync(PartSearchCriteria criteria)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var query = db.Table<PartModel>();

            // Apply the same filters as SearchAsync
            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                var searchTerm = criteria.SearchTerm.ToLowerInvariant();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm) ||
                    p.Manufacturer.ToLower().Contains(searchTerm));
            }

            // ... other filters ...

            return await query.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total count for criteria {@Criteria}", criteria);
            throw new RepositoryException("Error getting total count", ex);
        }
    }

    public async Task<Part> AddAsync(Part part)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var model = part.ToModel();
            await db.InsertAsync(model);
            return model.ToDomain();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding part {@Part}", part);
            throw new RepositoryException("Error adding part", ex);
        }
    }

    public async Task<Part> UpdateAsync(Part part)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var model = part.ToModel();
            await db.UpdateAsync(model);
            return model.ToDomain();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating part {@Part}", part);
            throw new RepositoryException("Error updating part", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var result = await db.DeleteAsync<PartModel>(id);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting part with ID {Id}", id);
            throw new RepositoryException($"Error deleting part with ID {id}", ex);
        }
    }

    private static IQueryable<PartModel> ApplySorting(
        IQueryable<PartModel> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy.ToLowerInvariant() switch
        {
            "name" => sortDescending 
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "price" => sortDescending
                ? query.OrderByDescending(p => p.PriceAmount)
                : query.OrderBy(p => p.PriceAmount),
            "manufacturer" => sortDescending
                ? query.OrderByDescending(p => p.Manufacturer)
                : query.OrderBy(p => p.Manufacturer),
            _ => query.OrderBy(p => p.Id)
        };
    }

    public async Task<IEnumerable<Part>> GetByIdsAsync(IEnumerable<int> ids)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var idList = ids.ToList();

            var models = await db.Table<PartModel>()
                .Where(p => idList.Contains(p.Id))
                .ToListAsync();

            return models.Select(m => m.ToDomain());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving parts with IDs {@Ids}", ids);
            throw new RepositoryException("Error retrieving parts", ex);
        }
    }
    
   public async Task<int> AddRangeAsync(IEnumerable<Part> parts)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var models = parts.Select(p => p.ToModel());

            return await db.RunInTransactionAsync<int>(async (connection) =>
            {
                var count = 0;
                foreach (var batch in models.Chunk(100))
                {
                    count += await connection.InsertAllAsync(batch);
                }
                return count;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding parts in batch");
            throw new RepositoryException("Error adding parts in batch", ex);
        }
    }

    public async Task<int> UpdateRangeAsync(IEnumerable<Part> parts)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var models = parts.Select(p => p.ToModel());

            return await db.RunInTransactionAsync<int>(async (connection) =>
            {
                var count = 0;
                foreach (var batch in models.Chunk(100))
                {
                    count += await connection.UpdateAllAsync(batch);
                }
                return count;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parts in batch");
            throw new RepositoryException("Error updating parts in batch", ex);
        }
    }

    public async Task<int> DeleteRangeAsync(IEnumerable<int> ids)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var idList = ids.ToList();

            return await db.RunInTransactionAsync<int>(async (connection) =>
            {
                return await connection.Table<PartModel>()
                    .Where(p => idList.Contains(p.Id))
                    .DeleteAsync();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parts with IDs {@Ids}", ids);
            throw new RepositoryException("Error deleting parts in batch", ex);
        }
    }    
}