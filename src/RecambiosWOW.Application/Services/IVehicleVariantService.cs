namespace RecambiosWOW.Application.Services;

public interface IVehicleVariantService
{
    Task<VehicleVariantDto> GetByIdAsync(int id);
    Task<VehicleVariantDto> GetByManufacturerCodeAsync(int modelId, string code);
    Task<IEnumerable<VehicleVariantDto>> GetByModelIdAsync(int modelId);
    Task<VehicleVariantDto> CreateAsync(CreateVehicleVariantDto dto);
    Task<VehicleVariantDto> UpdateAsync(UpdateVehicleVariantDto dto);
    Task<bool> DeleteAsync(int id);
}