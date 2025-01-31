using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecambiosWOW.Core.Interfaces.Services;
using RecambiosWOW.Infrastructure.Common.Settings;
using RecambiosWOW.Infrastructure.Providers.Database.Sqlite.Database.StartUp;
using SQLite;

namespace RecambiosWOW.Infrastructure.Services.Database.Startup;

public class DatabaseStartupService(
    IOptions<DatabaseSettings> settings,
    ILogger<SqliteStartupService> logger)
    : IStartupService
{
    private readonly DatabaseSettings _settings = settings.Value;
    private static SQLiteAsyncConnection? _database;

    public int Order => 1; // Ensure database initializes first

    private static SQLiteAsyncConnection Database => _database ?? 
                                                     throw new InvalidOperationException("Database not initialized");

    public async Task InitializeAsync()
    {
        try
        {
            logger.LogInformation("Initializing SQLite database...");

            var databasePath = _settings.DatabasePath;
            if (string.IsNullOrEmpty(databasePath))
            {
                databasePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RecambiosWow.db3");
            }

            _database = new SQLiteAsyncConnection(databasePath);

            if (_settings.AutoCreateTables)
            {
                await CreateTablesAsync();
            }

            logger.LogInformation("Database initialized successfully at: {DatabasePath}", databasePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error initializing database");
            throw;
        }
    }

    private async Task CreateTablesAsync()
    {
        logger.LogInformation("Creating database tables...");
        await Database.CreateTablesAsync(CreateFlags.None
            // Add your entity types here
            // typeof(Product), typeof(Category), etc.
        );
    }
}
