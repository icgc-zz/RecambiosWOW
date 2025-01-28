using RecambiosWOW.Application.DTOs;
using RecambiosWOW.Application.DTOs.Search;

namespace RecambiosWOW.Application.Interfaces;

public interface IPartService
{
    Task<PartDto> GetByIdAsync(int id);
    Task<PartDto> GetBySerialNumberAsync(string serialNumber);
    Task<IEnumerable<PartDto>> SearchAsync(PartSearchCriteriaDto criteria);
    Task<PartDto> CreateAsync(PartDto partDto);
    Task<PartDto> UpdateAsync(PartDto partDto);
    Task<bool> DeleteAsync(int id);
}
