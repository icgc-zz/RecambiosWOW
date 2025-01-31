namespace RecambiosWOW.Core.Interfaces.Repositories;

/// <summary>
/// Base repository interface with common CRUD operations
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<int> AddAsync(TEntity entity);
    Task<int> UpdateAsync(TEntity entity);
    Task<int> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
