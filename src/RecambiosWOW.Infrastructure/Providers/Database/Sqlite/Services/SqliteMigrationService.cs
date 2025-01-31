// Infrastructure/Providers/Database/Sqlite/Services/SqliteMigrationService.cs

using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Interfaces.Providers.Database.Migrations;
using RecambiosWOW.Infrastructure.Providers.Database.Sqlite;
using RecambiosWOW.Infrastructure.Services.Database.Migrations;
using SQLite;

public class SqliteMigrationService : IMigrationService
{
   private readonly ISqliteDbProvider _dbProvider;
   private readonly ILogger<SqliteMigrationService> _logger;
   private const string MigrationTableName = "__Migrations";

   public SqliteMigrationService(
       ISqliteDbProvider dbProvider,
       ILogger<SqliteMigrationService> logger)
   {
       _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
       _logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }

   public async Task<int> GetCurrentVersionAsync()
   {
       var connection = await _dbProvider.GetSqliteConnectionAsync();
       await EnsureMigrationTableExistsAsync(connection);

       return await connection.ExecuteScalarAsync<int>(
           $"SELECT COALESCE(MAX(Version), 0) FROM {MigrationTableName}");
   }

   public async Task<bool> MigrateAsync(CancellationToken cancellationToken = default)
   {
       var connection = await _dbProvider.GetSqliteConnectionAsync();
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
       var connection = await _dbProvider.GetSqliteConnectionAsync();
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

   private async Task EnsureMigrationTableExistsAsync(SQLiteAsyncConnection connection)
   {
       await connection.ExecuteAsync($@"
           CREATE TABLE IF NOT EXISTS {MigrationTableName} (
               Version INTEGER PRIMARY KEY,
               Description TEXT NOT NULL,
               AppliedOn DATETIME NOT NULL
           )");
   }

   private async Task ApplyMigrationAsync(SQLiteAsyncConnection connection, IMigration<SQLiteAsyncConnection> migration)
   {
       await connection.RunInTransactionAsync(async () =>
       {
           await migration.UpAsync(connection);
           await connection.ExecuteAsync(
               $"INSERT INTO {MigrationTableName} (Version, Description, AppliedOn) VALUES (?, ?, ?)",
               migration.Version,
               migration.Description,
               DateTime.UtcNow);
       });
   }

   private async Task RollbackMigrationAsync(SQLiteAsyncConnection connection, IMigration<SQLiteAsyncConnection> migration)
   {
       await connection.RunInTransactionAsync(async () =>
       {
           await migration.DownAsync(connection);
           await connection.ExecuteAsync(
               $"DELETE FROM {MigrationTableName} WHERE Version = ?",
               migration.Version);
       });
   }

   private IEnumerable<IMigration<SQLiteAsyncConnection>> GetMigrations()
   {
       return new IMigration<SQLiteAsyncConnection>[]
       {
           new InitialMigration(),
           new AddIndexesMigration()
       };
   }
}