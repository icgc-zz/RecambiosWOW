namespace RecambiosWOW.Core.Interfaces.Repositories;

/// <summary>
/// Write-only repository interface for handling command operations
/// </summary>
public interface IWriteRepository<TEntity> where TEntity : class
{
    Task<int> AddAsync(TEntity entity);
    Task<int> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<int> UpdateAsync(TEntity entity);
    Task<int> DeleteAsync(int id);
    Task<int> DeleteRangeAsync(IEnumerable<int> ids);
}