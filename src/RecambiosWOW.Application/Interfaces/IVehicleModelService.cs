using RecambiosWOW.Application.DTOs;
using RecambiosWOW.Application.DTOs.Search;
using RecambiosWOW.Core.Domain.Criteria;

namespace RecambiosWOW.Application.Interfaces;

public interface IVehicleModelService
{
    Task<VehicleModelDto> GetByIdAsync(int id);
    Task<VehicleModelDto> GetByDetailsAsync(string make, string model, int year);
    Task<IEnumerable<VehicleModelDto>> SearchAsync(VehicleModelSearchCriteria criteria);
    Task<VehicleModelDto> CreateAsync(CreateVehicleModelDto dto);
    Task<VehicleModelDto> UpdateAsync(UpdateVehicleModelDto dto);
    Task<bool> DeleteAsync(int id);
}