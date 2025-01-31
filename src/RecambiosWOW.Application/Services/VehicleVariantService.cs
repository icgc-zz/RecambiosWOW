using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.ValueObjects;
using RecambiosWOW.Core.Interfaces.Repositories;
using EngineDetails = RecambiosWOW.Core.Domain.Entities.EngineDetails;
using IVehicleModelRepository = RecambiosWOW.Core.Interfaces.Repositories.IVehicleModelRepository;

namespace RecambiosWOW.Application.Services;

public class VehicleVariantService : IVehicleVariantService
{
    private readonly IVehicleVariantRepository _variantRepository;
    private readonly IVehicleModelRepository _modelRepository;
    private readonly ILogger<VehicleVariantService> _logger;

    public VehicleVariantService(
        IVehicleVariantRepository variantRepository,
        IVehicleModelRepository modelRepository,
        ILogger<VehicleVariantService> logger)
    {
        _variantRepository = variantRepository ?? throw new ArgumentNullException(nameof(variantRepository));
        _modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VehicleVariantDto> GetByIdAsync(int id)
    {
        var variant = await _variantRepository.GetByIdAsync(id);
        return MapToDto(variant);
    }

    public async Task<VehicleVariantDto> GetByManufacturerCodeAsync(int modelId, string code)
    {
        var variant = await _variantRepository.GetByManufacturerCodeAsync(modelId, code);
        return MapToDto(variant);
    }

    public async Task<IEnumerable<VehicleVariantDto>> GetByModelIdAsync(int modelId)
    {
        var variants = await _variantRepository.GetByModelIdAsync(modelId);
        return variants.Select(MapToDto);
    }

    public async Task<VehicleVariantDto> CreateAsync(CreateVehicleVariantDto dto)
    {
        // Verify the model exists
        var model = await _modelRepository.GetByIdAsync(dto.ModelId);

        var variant = new VehicleVariant(
            model,
            dto.ManufacturerCode,
            new EngineDetails(
                dto.Engine.Type,
                dto.Engine.Displacement,
                dto.Engine.HorsePower,
                dto.Engine.FuelType,
                dto.Engine.EmissionStandard),
            new Dimensions(
                dto.Dimensions.Length,
                dto.Dimensions.Width,
                dto.Dimensions.Height,
                dto.Dimensions.WheelBase),
            new VehicleDetails(
                dto.Details.BodyType,
                dto.Details.Doors,
                dto.Details.Seats,
                dto.Details.DriveType,
                dto.Details.TransmissionType));

        var created = await _variantRepository.AddAsync(variant);
        return MapToDto(created);
    }

    public async Task<VehicleVariantDto> UpdateAsync(UpdateVehicleVariantDto dto)
    {
        var existing = await _variantRepository.GetByIdAsync(dto.Id);

        var updated = new VehicleVariant(
            existing.Model, // Keep the existing model
            dto.ManufacturerCode,
            new EngineDetails(
                dto.Engine.Type,
                dto.Engine.Displacement,
                dto.Engine.HorsePower,
                dto.Engine.FuelType,
                dto.Engine.EmissionStandard),
            new Dimensions(
                dto.Dimensions.Length,
                dto.Dimensions.Width,
                dto.Dimensions.Height,
                dto.Dimensions.WheelBase),
            new VehicleDetails(
                dto.Details.BodyType,
                dto.Details.Doors,
                dto.Details.Seats,
                dto.Details.DriveType,
                dto.Details.TransmissionType));

        var result = await _variantRepository.UpdateAsync(updated);
        return MapToDto(result);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _variantRepository.DeleteAsync(id);
    }

    private static VehicleVariantDto MapToDto(VehicleVariant variant)
    {
        return new VehicleVariantDto
        {
            Id = variant.Id,
            ModelId = variant.Model.Id,
            ManufacturerCode = variant.ManufacturerCode,
            Engine = new EngineDetailsDto
            {
                Type = variant.Engine.Type,
                Displacement = variant.Engine.Displacement,
                HorsePower = variant.Engine.HorsePower,
                FuelType = variant.Engine.FuelType,
                EmissionStandard = variant.Engine.EmissionStandard
            },
            Dimensions = new DimensionsDto
            {
                Length = variant.Dimensions.Length,
                Width = variant.Dimensions.Width,
                Height = variant.Dimensions.Height,
                WheelBase = variant.Dimensions.WheelBase
            },
            Details = new VehicleDetailsDto
            {
                BodyType = variant.Details.BodyType,
                Doors = variant.Details.Doors,
                Seats = variant.Details.Seats,
                DriveType = variant.Details.DriveType,
                TransmissionType = variant.Details.TransmissionType
            },
            CreatedAt = variant.AuditInfo.CreatedAt,
            UpdatedAt = variant.AuditInfo.UpdatedAt
        };
    }
}