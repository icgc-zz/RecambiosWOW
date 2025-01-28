using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Interfaces.Database;

namespace RecambiosWOW.Infrastructure.Services.Database.Migrations;

public class SqliteMigrationService : IMigrationService
{
    private readonly IDbProvider _dbProvider;
    private readonly ILogger<SqliteMigrationService> _logger;
    private const string MigrationTableName = "__Migrations";

    public SqliteMigrationService(
        IDbProvider dbProvider,
        ILogger<SqliteMigrationService> logger)
    {
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> GetCurrentVersionAsync()
    {
        await using var connection = await _dbProvider.GetConnectionAsync();
        await EnsureMigrationTableExistsAsync(connection);

        var version = await connection.ExecuteScalarAsync<int>(
            $"SELECT COALESCE(MAX(Version), 0) FROM {MigrationTableName}");
        
        return version;
    }

    public async Task<bool> MigrateAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _dbProvider.GetConnectionAsync();
        await EnsureMigrationTableExistsAsync(connection);

        var currentVersion = await GetCurrentVersionAsync();
        var migrations = GetMigrations()
            .Where(m => m.Version > currentVersion)
            .OrderBy(m => m.Version);

        foreach (var migration in migrations)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await ApplyMigrationAsync(connection, migration);
                _logger.LogInformation(
                    "Applied migration {Version}: {Description}",
                    migration.Version,
                    migration.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error applying migration {Version}: {Description}", 
                    migration.Version, 
                    migration.Description);
                throw;
            }
        }

        return true;
    }

    public async Task<bool> RollbackAsync(int targetVersion, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dbProvider.GetConnectionAsync();
        var currentVersion = await GetCurrentVersionAsync();

        var migrations = GetMigrations()
            .Where(m => m.Version <= currentVersion && m.Version > targetVersion)
            .OrderByDescending(m => m.Version);

        foreach (var migration in migrations)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await RollbackMigrationAsync(connection, migration);
                _logger.LogInformation(
                    "Rolled back migration {Version}: {Description}",
                    migration.Version,
                    migration.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error rolling back migration {Version}: {Description}", 
                    migration.Version, 
                    migration.Description);
                throw;
            }
        }

        return true;
    }

    private async Task EnsureMigrationTableExistsAsync(IDbConnection connection)
    {
        await connection.ExecuteAsync($@"
            CREATE TABLE IF NOT EXISTS {MigrationTableName} (
                Version INTEGER PRIMARY KEY,
                Description TEXT NOT NULL,
                AppliedOn DATETIME NOT NULL
            )");
    }

    private async Task ApplyMigrationAsync(IDbConnection connection, IMigration migration)
    {
        // Begin transaction
        await connection.ExecuteAsync("BEGIN TRANSACTION");
        
        try
        {
            // Apply the migration
            await migration.UpAsync(connection);

            // Record the migration
            await connection.ExecuteAsync(
                $"INSERT INTO {MigrationTableName} (Version, Description, AppliedOn) VALUES (@Version, @Description, @AppliedOn)",
                new
                {
                    Version = migration.Version,
                    Description = migration.Description,
                    AppliedOn = DateTime.UtcNow
                });

            // Commit transaction
            await connection.ExecuteAsync("COMMIT");
        }
        catch
        {
            // Rollback transaction on error
            await connection.ExecuteAsync("ROLLBACK");
            throw;
        }
    }

    private async Task RollbackMigrationAsync(IDbConnection connection, IMigration migration)
    {
        // Begin transaction
        await connection.ExecuteAsync("BEGIN TRANSACTION");
        
        try
        {
            // Rollback the migration
            await migration.DownAsync(connection);

            // Remove the migration record
            await connection.ExecuteAsync(
                $"DELETE FROM {MigrationTableName} WHERE Version = @Version",
                new { Version = migration.Version });

            // Commit transaction
            await connection.ExecuteAsync("COMMIT");
        }
        catch
        {
            // Rollback transaction on error
            await connection.ExecuteAsync("ROLLBACK");
            throw;
        }
    }

    private IEnumerable<IMigration> GetMigrations()
    {
        return new IMigration[]
        {
            new InitialMigration(),
            new AddIndexesMigration(),
            // Add more migrations here...
        };
    }
}