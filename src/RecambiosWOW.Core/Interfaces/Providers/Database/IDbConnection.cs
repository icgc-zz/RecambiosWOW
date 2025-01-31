namespace RecambiosWOW.Core.Interfaces.Providers.Database;

public interface IDbConnection : IAsyncDisposable
{
    Task<T?> QueryFirstOrDefaultAsync<T>(string query, object? parameters = null) where T : class, new();
    Task<List<T>> GetAllAsync<T>() where T : class, new();
    Task<T?> GetAsync<T>(int id) where T : class, new();
    Task<int> InsertAsync<T>(T entity) where T : class;
    Task<int> UpdateAsync<T>(T entity) where T : class;
    Task<int> DeleteAsync<T>(int id) where T : class, new();
    Task<List<T>> QueryAsync<T>(string query, object parameters = null) where T : class, new();
    Task<int> ExecuteAsync(string query, object? parameters = null);
    Task<T> ExecuteScalarAsync<T>(string query, object? parameters = null); 
}