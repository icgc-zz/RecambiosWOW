using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecambiosWOW.Core.Interfaces.Services;
using RecambiosWOW.Infrastructure.Common.Settings;
using SQLite;

namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite.Database.StartUp;

public class SqliteStartupService : IStartupService
{
    private readonly DatabaseSettings _settings;
    private readonly ILogger<SqliteStartupService> _logger;
    private static SQLiteAsyncConnection? _database;

    public int Order => 1; // Ensure database initializes first

    public static SQLiteAsyncConnection Database => _database ?? 
        throw new InvalidOperationException("Database not initialized");

    public SqliteStartupService(
        IOptions<DatabaseSettings> settings,
        ILogger<SqliteStartupService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Initializing SQLite database...");

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

            _logger.LogInformation("Database initialized successfully at: {DatabasePath}", databasePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing database");
            throw;
        }
    }

    private async Task CreateTablesAsync()
    {
        _logger.LogInformation("Creating database tables...");
        await Database.CreateTablesAsync(CreateFlags.None
            // Add your entity types here
            // typeof(Product), typeof(Category), etc.
        );
    }
}
