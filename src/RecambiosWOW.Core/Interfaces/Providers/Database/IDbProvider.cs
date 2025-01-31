namespace RecambiosWOW.Core.Interfaces.Providers.Database;

public interface IDbProvider
{
    Task<IDbConnection> GetConnectionAsync();
    Task InitializeDatabaseAsync();
    Task<bool> CheckDatabaseExistsAsync();
    string GetDatabasePath();
}
    
