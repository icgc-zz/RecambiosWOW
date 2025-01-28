using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Interfaces.Database;
using RecambiosWOW.Infrastructure.Data.Models;
using SQLite;

namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite;

public class SqliteDatabaseProvider(
    IConfiguration configuration,
    ILogger<SqliteDatabaseProvider> logger)
    : IDatabaseProvider
{
    private readonly string _databasePath = configuration.GetValue<string>("Database:Path") 
                                            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "recambioswow.db");
    private SQLiteAsyncConnection _connection;
    private readonly ILogger<SqliteDatabaseProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
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

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            var connection = await GetConnectionAsync();

            // Create tables
            await connection.CreateTableAsync<PartModel>();
            await connection.CreateTableAsync<VehicleModel>();

            // Create AlertThresholds table
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS AlertThresholds (
                    MetricName TEXT PRIMARY KEY,
                    Threshold REAL NOT NULL,
                    UpdatedAt DATETIME NOT NULL
                );");

            // Create SearchMetrics table
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS SearchMetrics (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    QueryText TEXT,
                    ResultCount INTEGER,
                    ExecutionTimeMs INTEGER,
                    PageSize INTEGER,
                    PageNumber INTEGER,
                    Timestamp DATETIME,
                    CacheHit BOOLEAN,
                    IndexSizeBytes INTEGER,
                    ConcurrentSearches INTEGER
                );");

            // Create Alerts table
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS Alerts (
                    Id TEXT PRIMARY KEY,
                    MetricName TEXT NOT NULL,
                    CurrentValue REAL NOT NULL,
                    Threshold REAL NOT NULL,
                    Message TEXT NOT NULL,
                    Severity INTEGER NOT NULL,
                    Timestamp DATETIME NOT NULL,
                    Acknowledged BOOLEAN NOT NULL DEFAULT 0
                );");

            _logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing database");
            throw;
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
}