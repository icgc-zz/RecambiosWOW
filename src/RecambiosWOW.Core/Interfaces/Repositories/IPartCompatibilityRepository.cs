using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IPartCompatibilityRepository
{
    Task<PartCompatibility> GetByIdAsync(int id);
    Task<IEnumerable<PartCompatibility>> GetByPartIdAsync(int partId);
    Task<IEnumerable<PartCompatibility>> GetByVehicleModelIdAsync(int vehicleModelId);
    Task<PartCompatibility> AddAsync(PartCompatibility compatibility);
    Task<PartCompatibility> UpdateAsync(PartCompatibility compatibility);
    Task<bool> DeleteAsync(int id);
}