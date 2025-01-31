using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IInventoryItemRepository
{
    Task<InventoryItem> GetByIdAsync(int id);
    Task<IEnumerable<InventoryItem>> GetByInventoryIdAsync(int inventoryId);
    Task<InventoryItem> AddAsync(InventoryItem item);
    Task<InventoryItem> UpdateAsync(InventoryItem item);
    Task<bool> DeleteAsync(int id);
}