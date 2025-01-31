using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IVehicleVariantRepository
{
    Task<VehicleVariant> GetByIdAsync(int id);
    Task<VehicleVariant> GetByManufacturerCodeAsync(int modelId, string code);
    Task<IEnumerable<VehicleVariant?>> GetByModelIdAsync(int modelId);
    Task<VehicleVariant> AddAsync(VehicleVariant variant);
    Task<VehicleVariant> UpdateAsync(VehicleVariant variant);
    Task<bool> DeleteAsync(int id);
}