using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IPartsWatchListRepository
{
    Task<PartsWatchList> GetByIdAsync(int id);
    Task<IEnumerable<PartsWatchList>> GetByMemberIdAsync(int memberId);
    Task<PartsWatchList> AddAsync(PartsWatchList watchList);
    Task<PartsWatchList> UpdateAsync(PartsWatchList watchList);
    Task<bool> DeleteAsync(int id);
}