using System.Data;

namespace RecambiosWOW.Core.Interfaces.Database;

public interface IDatabase
{
    Task<IDbConnection> GetConnectionAsync();
}