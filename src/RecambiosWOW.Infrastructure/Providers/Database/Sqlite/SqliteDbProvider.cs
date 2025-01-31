using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SQLite;

namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite;

public class SqliteDbProvider : ISqliteDbProvider
{
    private readonly string _databasePath;
    private SQLiteAsyncConnection _connection;
    private readonly ILogger<SqliteDbProvider> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public SqliteDbProvider(
        IConfiguration configuration,
        ILogger<SqliteDbProvider> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _databasePath = configuration.GetValue<string>("Database:Path") 
                        ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "recambioswow.db");
    }

    public async Task<SQLiteAsyncConnection> GetSqliteConnectionAsync()
    {
        await EnsureConnectionAsync();
        return _connection;
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection != null)
            return;

        await _semaphore.WaitAsync();
        try
        {
            if (_connection != null)
                return;

            _connection = new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            _logger.LogInformation("Created new database connection to {Path}", _databasePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database connection to {Path}", _databasePath);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<bool> CheckDatabaseExistsAsync()
    {
        return Task.FromResult(File.Exists(_databasePath));
    }

    public string GetDatabasePath()
    {
        return _databasePath;
    }

    public async Task InitializeDatabaseAsync()
    {
        var connection = await GetSqliteConnectionAsync();
        // Add any initialization logic here
    }
}