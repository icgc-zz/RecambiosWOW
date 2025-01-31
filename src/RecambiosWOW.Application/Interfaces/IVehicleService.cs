using RecambiosWOW.Application.DTOs;
using RecambiosWOW.Application.DTOs.Search;

namespace RecambiosWOW.Application.Interfaces;

public interface IVehicleService
{
    Task<VehicleDto> GetByIdAsync(int id);
    Task<VehicleDto> GetByVinAsync(string serialNumber);
    Task<IEnumerable<VehicleDto>> SearchAsync(VehicleSearchCriteriaDto criteria);
    Task<VehicleDto> CreateAsync(VehicleDto vehicleDto);
    Task<VehicleDto> UpdateAsync(VehicleDto vehicleDto);
    Task<bool> DeleteAsync(int id);
}
