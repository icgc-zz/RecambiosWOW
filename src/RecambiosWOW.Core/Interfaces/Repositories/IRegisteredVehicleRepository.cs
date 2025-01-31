using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IRegisteredVehicleRepository
{
    Task<RegisteredVehicle> GetByIdAsync(int id);
    Task<RegisteredVehicle> GetByVinAsync(string vin);
    Task<IEnumerable<RegisteredVehicle>> GetByOwnerIdAsync(int ownerId);
    Task<RegisteredVehicle> AddAsync(RegisteredVehicle vehicle);
    Task<RegisteredVehicle> UpdateAsync(RegisteredVehicle vehicle);
    Task<bool> DeleteAsync(int id);
}