using RecambiosWOW.Core.Domain.Models;

namespace RecambiosWOW.Core.Interfaces.Services;

public interface IVehicleDataProvider
{
    Task<VehicleInfo> GetByVinAsync(string vin, CancellationToken cancellationToken = default);
    Task<VehicleInfo> GetByPlateAsync(string plate, string countryCode, CancellationToken cancellationToken = default);
    Task<bool> ValidateVinAsync(string vin, CancellationToken cancellationToken = default);
}