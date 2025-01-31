using Microsoft.Extensions.Logging;
using RecambiosWOW.Application.DTOs;
using RecambiosWOW.Application.DTOs.Search;
using RecambiosWOW.Application.Interfaces;
using RecambiosWOW.Core.Domain.Criteria;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Application.Services;

public class VehicleModelService : IVehicleModelService
{
    private readonly IVehicleModelRepository _vehicleModelRepository;
    private readonly ILogger<VehicleModelService> _logger;

    public VehicleModelService(
        IVehicleModelRepository vehicleModelRepository,
        ILogger<VehicleModelService> logger)
    {
        _vehicleModelRepository = vehicleModelRepository ?? throw new ArgumentNullException(nameof(vehicleModelRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VehicleModelDto> GetByIdAsync(int id)
    {
        var model = await _vehicleModelRepository.GetByIdAsync(id);
        return MapToDto(model);
    }

    public async Task<VehicleModelDto> GetByDetailsAsync(string make, string model, int year)
    {
        var vehicleModel = await _vehicleModelRepository.GetByDetailsAsync(make, model, year);
        return MapToDto(vehicleModel);
    }

    public async Task<IEnumerable<VehicleModelDto>> SearchAsync(VehicleModelSearchCriteria criteria)
    {
        var models = await _vehicleModelRepository.SearchAsync(criteria);
        return models.Select(MapToDto);
    }

    public async Task<VehicleModelDto> CreateAsync(CreateVehicleModelDto dto)
    {
        var model = new VehicleModel(
            dto.Make,
            dto.Model,
            dto.Year);

        var created = await _vehicleModelRepository.AddAsync(model);
        return MapToDto(created);
    }

    public async Task<VehicleModelDto> UpdateAsync(UpdateVehicleModelDto dto)
    {
        var existing = await _vehicleModelRepository.GetByIdAsync(dto.Id);

        // You might want to create an Update method in the VehicleModel class
        // instead of creating a new instance
        var updated = new VehicleModel(
            dto.Make,
            dto.Model,
            dto.Year);

        var result = await _vehicleModelRepository.UpdateAsync(updated);
        return MapToDto(result);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _vehicleModelRepository.DeleteAsync(id);
    }

    private static VehicleModelDto MapToDto(VehicleModel model)
    {
        return new VehicleModelDto
        {
            Id = model.Id,
            Make = model.Make,
            Model = model.Model,
            Year = model.Year,
            Variants = model.Variants.Select(v => new VehicleVariantDto
            {
                Id = v.Id,
                ManufacturerCode = v.ManufacturerCode,
                Engine = new EngineDetailsDto
                {
                    Type = v.Engine.Type,
                    Displacement = v.Engine.Displacement,
                    HorsePower = v.Engine.HorsePower,
                    FuelType = v.Engine.FuelType,
                    EmissionStandard = v.Engine.EmissionStandard
                },
                Dimensions = new DimensionsDto
                {
                    Length = v.Dimensions.Length,
                    Width = v.Dimensions.Width,
                    Height = v.Dimensions.Height,
                    WheelBase = v.Dimensions.WheelBase
                },
                Details = new VehicleDetailsDto
                {
                    BodyType = v.Details.BodyType,
                    Doors = v.Details.Doors,
                    Seats = v.Details.Seats,
                    DriveType = v.Details.DriveType,
                    TransmissionType = v.Details.TransmissionType
                }
            }).ToList(),
            CreatedAt = model.AuditInfo.CreatedAt,
            UpdatedAt = model.AuditInfo.UpdatedAt
        };
    }
}