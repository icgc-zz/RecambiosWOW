using System.Linq.Expressions;

namespace RecambiosWOW.Core.Interfaces.Repositories;

/// <summary>
/// Read-only repository interface for handling query operations
/// </summary>
public interface IReadRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
}
