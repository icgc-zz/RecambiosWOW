using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Criteria;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Exceptions;
using RecambiosWOW.Core.Interfaces.Providers.Database;
using RecambiosWOW.Core.Interfaces.Repositories;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite.Repositories;

public class SqliteVehicleModelRepository : IVehicleModelRepository
{
    private readonly IDbProvider _dbProvider;
    private readonly ILogger<SqliteVehicleModelRepository> _logger;

    public SqliteVehicleModelRepository(
        IDbProvider dbProvider,
        ILogger<SqliteVehicleModelRepository> logger)
    {
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VehicleModel> GetByIdAsync(int id)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var model = await connection.GetAsync<VehicleModel>(id);
            
            if (model == null)
                throw new NotFoundException($"VehicleModel with ID {id} not found");

            await LoadVariantsAsync(connection, model);
            return model;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error getting vehicle model with ID {Id}", id);
            throw;
        }
    }

    public async Task<VehicleModel> GetByDetailsAsync(string make, string model, int year)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var vehicleModel = await connection.QueryFirstOrDefaultAsync<VehicleModel>(
                "SELECT * FROM VehicleModels WHERE Make = @Make AND Model = @Model AND Year = @Year",
                new { Make = make, Model = model, Year = year });

            if (vehicleModel == null)
                throw new NotFoundException($"VehicleModel {make} {model} {year} not found");

            await LoadVariantsAsync(connection, vehicleModel);
            return vehicleModel;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error getting vehicle model {Make} {Model} {Year}", make, model, year);
            throw;
        }
    }

    public async Task<IEnumerable<VehicleModel>> SearchAsync(VehicleModelSearchCriteria criteria)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            
            var query = new StringBuilder("SELECT * FROM VehicleModels WHERE 1=1");
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                query.Append(" AND (Make LIKE @SearchTerm OR Model LIKE @SearchTerm)");
                parameters["SearchTerm"] = $"%{criteria.SearchTerm}%";
            }

            if (!string.IsNullOrWhiteSpace(criteria.Make))
            {
                query.Append(" AND Make = @Make");
                parameters["Make"] = criteria.Make;
            }

            if (!string.IsNullOrWhiteSpace(criteria.Model))
            {
                query.Append(" AND Model = @Model");
                parameters["Model"] = criteria.Model;
            }

            if (criteria.Year.HasValue)
            {
                query.Append(" AND Year = @Year");
                parameters["Year"] = criteria.Year.Value;
            }
            else
            {
                if (criteria.YearFrom.HasValue)
                {
                    query.Append(" AND Year >= @YearFrom");
                    parameters["YearFrom"] = criteria.YearFrom.Value;
                }

                if (criteria.YearTo.HasValue)
                {
                    query.Append(" AND Year <= @YearTo");
                    parameters["YearTo"] = criteria.YearTo.Value;
                }
            }

            // Add sorting
            if (!string.IsNullOrWhiteSpace(criteria.SortBy))
            {
                query.Append($" ORDER BY {criteria.SortBy} {(criteria.SortDescending ? "DESC" : "ASC")}");
            }
            else
            {
                query.Append(" ORDER BY Make, Model, Year");
            }

            // Add pagination
            query.Append(" LIMIT @Take OFFSET @Skip");
            parameters["Take"] = criteria.Take;
            parameters["Skip"] = criteria.Skip;

            var models = await connection.QueryAsync<VehicleModel>(query.ToString(), parameters);

            if (criteria.IncludeVariants)
            {
                foreach (var model in models)
                {
                    await LoadVariantsAsync(connection, model);
                }
            }

            return models;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching vehicle models with criteria {@Criteria}", criteria);
            throw;
        }
    }

    public async Task<int> GetTotalCountAsync(VehicleModelSearchCriteria criteria)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            
            var query = new StringBuilder("SELECT COUNT(*) FROM VehicleModels WHERE 1=1");
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                query.Append(" AND (Make LIKE @SearchTerm OR Model LIKE @SearchTerm)");
                parameters["SearchTerm"] = $"%{criteria.SearchTerm}%";
            }

            if (!string.IsNullOrWhiteSpace(criteria.Make))
            {
                query.Append(" AND Make = @Make");
                parameters["Make"] = criteria.Make;
            }

            if (!string.IsNullOrWhiteSpace(criteria.Model))
            {
                query.Append(" AND Model = @Model");
                parameters["Model"] = criteria.Model;
            }

            if (criteria.Year.HasValue)
            {
                query.Append(" AND Year = @Year");
                parameters["Year"] = criteria.Year.Value;
            }
            else
            {
                if (criteria.YearFrom.HasValue)
                {
                    query.Append(" AND Year >= @YearFrom");
                    parameters["YearFrom"] = criteria.YearFrom.Value;
                }

                if (criteria.YearTo.HasValue)
                {
                    query.Append(" AND Year <= @YearTo");
                    parameters["YearTo"] = criteria.YearTo.Value;
                }
            }

            return await connection.ExecuteScalarAsync<int>(query.ToString(), parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total count for criteria {@Criteria}", criteria);
            throw;
        }
    }

    public async Task<VehicleModel> AddAsync(VehicleModel model)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            await connection.InsertAsync(model);
            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding vehicle model {@Model}", model);
            throw;
        }
    }

    public async Task<VehicleModel> UpdateAsync(VehicleModel model)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            await connection.UpdateAsync(model);
            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle model {@Model}", model);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            await using var connection = await _dbProvider.GetConnectionAsync();
            var result = await connection.DeleteAsync<VehicleModel>(id);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle model with ID {Id}", id);
            throw;
        }
    }

    private async Task LoadVariantsAsync(IDbConnection connection, VehicleModel model)
    {
        var variants = await connection.QueryAsync<VehicleVariant>(
            "SELECT * FROM VehicleVariants WHERE ModelId = @ModelId",
            new { ModelId = model.Id });

        typeof(VehicleModel)
            .GetField("_variants", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(model, variants.ToList());
    }
}