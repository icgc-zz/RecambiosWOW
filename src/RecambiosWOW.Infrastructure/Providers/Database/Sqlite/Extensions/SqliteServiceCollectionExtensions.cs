using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecambiosWOW.Core.Interfaces.Providers.Database.Migrations;
using RecambiosWOW.Infrastructure.Services.Database.Startup;

namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite.Extensions;

public static class SqliteServiceCollectionExtensions
{
    public static IServiceCollection AddSqliteDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ISqliteDbProvider, SqliteDbProvider>();
        services.AddScoped<IMigrationService, SqliteMigrationService>();
        services.AddHostedService<DatabaseInitializationService>();

        return services;
    }
}