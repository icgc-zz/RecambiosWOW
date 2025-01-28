namespace RecambiosWOW.Core.Interfaces.Database;

public interface IDbProvider
{
    Task<IDbConnection> GetConnectionAsync();
    Task InitializeDatabaseAsync();
    Task<bool> CheckDatabaseExistsAsync();
    string GetDatabasePath();
}
    
