using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Exceptions;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Interfaces.Database;
using RecambiosWOW.Core.Search;
using RecambiosWOW.Infrastructure.Data.Models;

namespace RecambiosWOW.Infrastructure.Data.Repositories;

public class SqliteVehicleRepository : IVehicleRepository
{
    private readonly IDbProvider _dbProvider;
    private readonly ILogger<SqliteVehicleRepository> _logger;

    public SqliteVehicleRepository(
        IDbProvider dbProvider,
        ILogger<SqliteVehicleRepository> logger)
    {
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Vehicle> GetByVinAsync(string vin)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var vehicleModel = await db.Table<VehicleModel>()
                .FirstOrDefaultAsync(v => v.VIN == vin);

            if (vehicleModel == null)
            {
                _logger.LogWarning("Vehicle with VIN {VIN} not found", vin);
                throw new NotFoundException($"Vehicle with VIN {vin} not found");
            }

            return vehicleModel.ToDomain();
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error retrieving vehicle with VIN {VIN}", vin);
            throw new RepositoryException($"Error retrieving vehicle with VIN {vin}", ex);
        }
    }

    public async Task<Vehicle> GetByPlateAsync(string plate)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var vehicleModel = await db.Table<VehicleModel>()
                .FirstOrDefaultAsync(v => v.LicensePlate == plate);

            if (vehicleModel == null)
            {
                _logger.LogWarning("Vehicle with plate {Plate} not found", plate);
                throw new NotFoundException($"Vehicle with plate {plate} not found");
            }

            return vehicleModel.ToDomain();
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error retrieving vehicle with plate {Plate}", plate);
            throw new RepositoryException($"Error retrieving vehicle with plate {plate}", ex);
        }
    }

    public async Task<IEnumerable<Vehicle>> SearchAsync(VehicleSearchCriteria criteria)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var query = db.Table<VehicleModel>();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                var searchTerm = criteria.SearchTerm.ToLowerInvariant();
                query = query.Where(v => 
                    v.Make.ToLower().Contains(searchTerm) ||
                    v.Model.ToLower().Contains(searchTerm) ||
                    v.VIN.ToLower().Contains(searchTerm) ||
                    v.LicensePlate.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Make))
            {
                query = query.Where(v => v.Make == criteria.Make);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Model))
            {
                query = query.Where(v => v.Model == criteria.Model);
            }

            if (criteria.Year.HasValue)
            {
                query = query.Where(v => v.Year == criteria.Year.Value);
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

            return results.Select(v => v.ToDomain());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching vehicles with criteria {@Criteria}", criteria);
            throw new RepositoryException("Error searching vehicles", ex);
        }
    }

    public async Task<int> GetTotalCountAsync(VehicleSearchCriteria criteria)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var query = db.Table<VehicleModel>();

            // Apply the same filters as SearchAsync
            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                var searchTerm = criteria.SearchTerm.ToLowerInvariant();
                query = query.Where(v => 
                    v.Make.ToLower().Contains(searchTerm) ||
                    v.Model.ToLower().Contains(searchTerm) ||
                    v.VIN.ToLower().Contains(searchTerm) ||
                    v.LicensePlate.ToLower().Contains(searchTerm));
            }

            // ... other filters same as SearchAsync ...

            return await query.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total count for criteria {@Criteria}", criteria);
            throw new RepositoryException("Error getting total count", ex);
        }
    }

    public async Task<Vehicle> AddAsync(Vehicle vehicle)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var model = vehicle.ToModel();
            await db.InsertAsync(model);
            return model.ToDomain();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding vehicle {@Vehicle}", vehicle);
            throw new RepositoryException("Error adding vehicle", ex);
        }
    }

    public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var model = vehicle.ToModel();
            await db.UpdateAsync(model);
            return model.ToDomain();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle {@Vehicle}", vehicle);
            throw new RepositoryException("Error updating vehicle", ex);
        }
    }

    public async Task<bool> DeleteAsync(string vin)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var result = await db.Table<VehicleModel>()
                .Where(v => v.VIN == vin)
                .DeleteAsync();
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle with VIN {VIN}", vin);
            throw new RepositoryException($"Error deleting vehicle with VIN {vin}", ex);
        }
    }

    private static IQueryable<VehicleModel> ApplySorting(
        IQueryable<VehicleModel> query,
        string sortBy,
        bool sortDescending)
    {
        return sortBy.ToLowerInvariant() switch
        {
            "make" => sortDescending 
                ? query.OrderByDescending(v => v.Make)
                : query.OrderBy(v => v.Make),
            "model" => sortDescending
                ? query.OrderByDescending(v => v.Model)
                : query.OrderBy(v => v.Model),
            "year" => sortDescending
                ? query.OrderByDescending(v => v.Year)
                : query.OrderBy(v => v.Year),
            _ => query.OrderBy(v => v.VIN)
        };
    }

    // Transaction handling example
    public async Task<bool> TransferVehicleDataAsync(string sourceVin, string targetVin)
    {
        var db = await _dbProvider.GetConnectionAsync();
        await db.RunInTransactionAsync(async (connection) =>
        {
            var sourceVehicle = await connection.Table<VehicleModel>()
                .FirstOrDefaultAsync(v => v.VIN == sourceVin);
            
            if (sourceVehicle == null)
                throw new NotFoundException($"Source vehicle with VIN {sourceVin} not found");

            var targetVehicle = await connection.Table<VehicleModel>()
                .FirstOrDefaultAsync(v => v.VIN == targetVin);
            
            if (targetVehicle == null)
                throw new NotFoundException($"Target vehicle with VIN {targetVin} not found");

            // Perform the transfer operations
            targetVehicle.SpecificationsJson = sourceVehicle.SpecificationsJson;
            
            await connection.UpdateAsync(targetVehicle);
        });

        return true;
    }
    
    public async Task<IEnumerable<Vehicle>> GetByIdsAsync(IEnumerable<int> ids)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var idList = ids.ToList();
            
            var models = await db.Table<VehicleModel>()
                .Where(p => idList.Contains(p.Id))
                .ToListAsync();

            return models.Select(m => m.ToDomain());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicles with IDs {@Ids}", ids);
            throw new RepositoryException("Error retrieving vehicles", ex);
        }
    }

    public async Task<int> AddRangeAsync(IEnumerable<Vehicle> vehicles)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var models = vehicles.Select(p => p.ToModel());

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
            _logger.LogError(ex, "Error adding vehicles in batch");
            throw new RepositoryException("Error adding vehicles in batch", ex);
        }
    }

    public async Task<int> UpdateRangeAsync(IEnumerable<Vehicle> vehicles)
    {
        try
        {
            var db = await _dbProvider.GetConnectionAsync();
            var models = vehicles.Select(p => p.ToModel());

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
            _logger.LogError(ex, "Error updating vehicles in batch");
            throw new RepositoryException("Error updating vehicles in batch", ex);
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
                return await connection.Table<VehicleModel>()
                    .Where(p => idList.Contains(p.Id))
                    .DeleteAsync();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicles with IDs {@Ids}", ids);
            throw new RepositoryException("Error deleting vehicles in batch", ex);
        }
    }    
}