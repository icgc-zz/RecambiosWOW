using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Interfaces.Database;
using SQLite;

namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite;

public class SqliteConnection : IDbConnection
{
    private readonly SQLiteAsyncConnection _connection;
    private readonly ILogger<SqliteConnection> _logger;

    public SqliteConnection(SQLiteAsyncConnection connection, ILogger<SqliteConnection> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T> QueryFirstOrDefaultAsync<T>(string query, object? parameters = null)
    {
        try
        {
            return await _connection.QueryFirstOrDefaultAsync<T>(query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query: {Query}", query);
            throw;
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string query, object parameters = null)
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

    public async Task<int> ExecuteAsync(string query, object parameters = null)
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

    public async Task<T> ExecuteScalarAsync<T>(string query, object parameters = null)
    {
        try
        {
            return await _connection.ExecuteScalarAsync<T>(query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing scalar query: {Query}", query);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.CloseAsync();
    }
}

// Infrastructure/Providers/Database/SqliteDatabaseProvider.cs
public class SqliteDatabaseProvider : IDbProvider
{
    private readonly string _databasePath;
    private SQLiteAsyncConnection _connection;
    private readonly ILogger<SqliteDatabaseProvider> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public SqliteDatabaseProvider(
        IConfiguration configuration,
        ILogger<SqliteDatabaseProvider> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        
        _databasePath = configuration.GetValue<string>("Database:Path") 
                        ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "recambioswow.db");
    }

    public async Task<IDbConnection> GetConnectionAsync()
    {
        await EnsureConnectionAsync();
        return new SqliteConnection(_connection, _loggerFactory.CreateLogger<SqliteConnection>());
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
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS AlertThresholds (
                    MetricName TEXT PRIMARY KEY,
                    Threshold REAL NOT NULL,
                    UpdatedAt DATETIME NOT NULL
                );");

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