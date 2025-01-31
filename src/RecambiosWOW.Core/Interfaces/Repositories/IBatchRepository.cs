namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IBatchRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<int> ids);
    Task<int> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities);
    Task<int> DeleteRangeAsync(IEnumerable<int> ids);
}