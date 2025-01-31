using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IMemberInventoryRepository
{
    Task<MemberInventory> GetByIdAsync(int id);
    Task<IEnumerable<MemberInventory>> GetByMemberIdAsync(int memberId);
    Task<MemberInventory> AddAsync(MemberInventory inventory);
    Task<MemberInventory> UpdateAsync(MemberInventory inventory);
    Task<bool> DeleteAsync(int id);
}