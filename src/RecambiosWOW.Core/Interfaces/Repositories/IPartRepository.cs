using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IPartRepository
{
    Task<Part> GetByIdAsync(int id);
    Task<Part> GetByIdentifierAsync(string manufacturer, string partNumber);
    Task<Part> GetBySerialNumberAsync(string serialNumber);
    Task<IEnumerable<Part>> SearchAsync(PartSearchCriteria criteria);
    Task<int> GetTotalCountAsync(PartSearchCriteria criteria);
    Task<Part> AddAsync(Part part);
    Task<Part> UpdateAsync(Part part);
    Task<bool> DeleteAsync(int id);
}