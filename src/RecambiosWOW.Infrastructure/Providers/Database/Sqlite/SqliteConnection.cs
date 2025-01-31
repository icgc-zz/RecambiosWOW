using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Interfaces.Providers.Database;
using SQLite;

namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite;

public class SqliteConnection(SQLiteAsyncConnection connection, ILogger<SqliteConnection> logger)
    : IDbConnection
{
    private readonly SQLiteAsyncConnection _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    private readonly ILogger<SqliteConnection> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string query, object? parameters = null) where T : class, new()
    {
        try
        {
            return await _connection.FindWithQueryAsync<T>(query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query: {Query}", query);
            throw;
        }
    }
    
    public async Task<List<T>> GetAllAsync<T>() where T : class, new()
    {
        try
        {
            return await _connection.Table<T>().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities of type {Type}", typeof(T).Name);
            throw;
        }
    }

    public async Task<T?> GetAsync<T>(int id) where T : class, new()
    {
        try
        {
            return await _connection.GetAsync<T>(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity of type {Type} with id {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public async Task<int> InsertAsync<T>(T entity) where T : class
    {
        try
        {
            return await _connection.InsertAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting entity of type {Type}", typeof(T).Name);
            throw;
        }
    }

    public async Task<int> UpdateAsync<T>(T entity) where T : class
    {
        try
        {
            return await _connection.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {Type}", typeof(T).Name);
            throw;
        }
    }

    public async Task<int> DeleteAsync<T>(int id) where T : class, new()
    {
        try
        {
            return await _connection.DeleteAsync<T>(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity of type {Type} with id {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public async Task<List<T>> QueryAsync<T>(string query, object parameters = null) where T : class, new()
    {
        try
        {
            return await _connection.QueryAsync<T>(query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query: {Query}", query);
            throw;
        }
    }

    public async Task<int> ExecuteAsync(string query, object? parameters = null)
    {
        try
        {
            return await _connection.ExecuteAsync(query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command: {Query}", query);
            throw;
        }
    }

    public Task<T> ExecuteScalarAsync<T>(string query, object? parameters = null)
    {
        try
        {
            return _connection.ExecuteScalarAsync<T>(query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command: {Query}", query);
            throw;
        }    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _connection.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing database connection");
            throw;
        }
    }
}