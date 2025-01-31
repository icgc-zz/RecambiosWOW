using RecambiosWOW.Core.Domain.Criteria;
using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Domain.ValueObjects;

public interface IVehicleModelRepository
{
    Task<VehicleModel> GetByIdAsync(int id);
    Task<VehicleModel> GetByDetailsAsync(string make, string model, int year);
    Task<IEnumerable<VehicleModel>> SearchAsync(VehicleModelSearchCriteria criteria);
    Task<int> GetTotalCountAsync(VehicleModelSearchCriteria criteria);
    Task<VehicleModel> AddAsync(VehicleModel model);
    Task<VehicleModel> UpdateAsync(VehicleModel model);
    Task<bool> DeleteAsync(int id);
}
