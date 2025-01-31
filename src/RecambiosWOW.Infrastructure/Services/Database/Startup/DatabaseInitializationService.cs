using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Interfaces.Providers.Database;

namespace RecambiosWOW.Infrastructure.Services.Database.Startup;

public class DatabaseInitializationService : IHostedService
{
    private readonly IDbProvider _databaseProvider;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(
        IDbProvider databaseProvider,
        ILogger<DatabaseInitializationService> logger)
    {
        _databaseProvider = databaseProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Initializing database...");
            await _databaseProvider.InitializeDatabaseAsync();
            _logger.LogInformation("Database initialization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database initialization");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}