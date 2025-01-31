using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Exceptions;
using RecambiosWOW.Core.Interfaces.Providers.Database;
using RecambiosWOW.Core.Interfaces.Repositories;
using VehicleModel = RecambiosWOW.Infrastructure.Data.Models.VehicleModel;

namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite.Repositories;

public class SqliteVehicleVariantRepository : IVehicleVariantRepository
{
    private readonly IDbProvider _dbProvider;
    private readonly ILogger<SqliteVehicleVariantRepository> _logger;

    public SqliteVehicleVariantRepository(
        IDbProvider dbProvider,
        ILogger<SqliteVehicleVariantRepository> logger)
    {
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VehicleVariant> GetByIdAsync(int id)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var variant = await connection.GetAsync<VehicleVariant>(id);
            
            if (variant == null)
                throw new NotFoundException($"VehicleVariant with ID {id} not found");

            await LoadRelationsAsync(connection, variant);
            return variant;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error getting vehicle variant with ID {Id}", id);
            throw;
        }
    }

    public async Task<VehicleVariant> GetByManufacturerCodeAsync(int modelId, string code)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var variant = await connection.QueryFirstOrDefaultAsync<VehicleVariant>(
                "SELECT * FROM VehicleVariants WHERE ModelId = @ModelId AND ManufacturerCode = @Code",
                new { ModelId = modelId, Code = code });

            if (variant == null)
                throw new NotFoundException($"VehicleVariant with code {code} for model {modelId} not found");

            await LoadRelationsAsync(connection, variant);
            return variant;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error getting vehicle variant with code {Code} for model {ModelId}", code, modelId);
            throw;
        }
    }

    public async Task<IEnumerable<VehicleVariant?>> GetByModelIdAsync(int modelId)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            List<VehicleVariant?> variants = await connection.QueryAsync<VehicleVariant>(
                "SELECT * FROM VehicleVariants WHERE ModelId = @ModelId ORDER BY ManufacturerCode",
                new { ModelId = modelId });

            foreach (var variant in variants)
            {
                await LoadRelationsAsync(connection, variant);
            }

            return variants;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vehicle variants for model {ModelId}", modelId);
            throw;
        }
    }

    public async Task<VehicleVariant> AddAsync(VehicleVariant variant)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            await connection.InsertAsync(variant);
            return variant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding vehicle variant {@Variant}", variant);
            throw;
        }
    }

    public async Task<VehicleVariant> UpdateAsync(VehicleVariant variant)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            await connection.UpdateAsync(variant);
            return variant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle variant {@Variant}", variant);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var result = await connection.DeleteAsync<VehicleVariant>(id);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle variant with ID {Id}", id);
            throw;
        }
    }

    private async Task LoadRelationsAsync(IDbConnection connection, VehicleVariant variant)
    {
        // Load the model
        variant = await connection.GetAsync<VehicleVariant>(variant.Id);
        
        // Load Engine, Dimensions, and Details
        // These would be stored in separate tables or as JSON in the database
        // Implementation depends on your storage strategy
    }
}