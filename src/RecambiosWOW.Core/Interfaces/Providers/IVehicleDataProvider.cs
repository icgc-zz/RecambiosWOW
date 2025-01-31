using RecambiosWOW.Core.Domain.Models;

namespace RecambiosWOW.Core.Interfaces.Providers;

public interface IVehicleDataProvider
{
    Task<VehicleInfo> GetByVinAsync(string vin);
    Task<VehicleInfo> GetByPlateAsync(string plate, string countryCode);
}