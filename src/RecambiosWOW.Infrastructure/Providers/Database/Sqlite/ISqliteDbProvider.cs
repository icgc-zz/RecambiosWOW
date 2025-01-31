namespace RecambiosWOW.Infrastructure.Providers.Database.Sqlite;

using SQLite;

public interface ISqliteDbProvider
{
    Task<SQLiteAsyncConnection> GetSqliteConnectionAsync();
    Task InitializeDatabaseAsync();
    Task<bool> CheckDatabaseExistsAsync();
    string GetDatabasePath();
}