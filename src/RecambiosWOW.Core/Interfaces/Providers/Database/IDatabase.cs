namespace RecambiosWOW.Core.Interfaces.Providers.Database;

public interface IDatabase
{
    Task<IDbConnection> GetConnectionAsync();
}