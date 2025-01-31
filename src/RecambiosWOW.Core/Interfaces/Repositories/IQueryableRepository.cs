using System.Linq.Expressions;

namespace RecambiosWOW.Core.Interfaces.Repositories;

/// <summary>
/// Interface for repositories that support advanced querying
/// </summary>
public interface IQueryableRepository<TEntity> where TEntity : class
{
    Task<IPagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");
        
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> filter,
        string includeProperties = "");
        
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter);
}
