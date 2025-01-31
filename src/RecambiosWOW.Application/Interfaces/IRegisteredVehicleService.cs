using RecambiosWOW.Application.DTOs;

namespace RecambiosWOW.Application.Interfaces;

public interface IRegisteredVehicleService
{
    Task<RegisteredVehicleDto> GetByIdAsync(int id);
    Task<RegisteredVehicleDto> GetByVinAsync(string vin);
    Task<IEnumerable<RegisteredVehicleDto>> GetByOwnerIdAsync(int ownerId);
    Task<RegisteredVehicleDto> CreateAsync(CreateRegisteredVehicleDto dto);
    Task<RegisteredVehicleDto> UpdateAsync(UpdateRegisteredVehicleDto dto);
    Task<bool> DeleteAsync(int id);
}