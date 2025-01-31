using RecambiosWOW.Application.DTOs;

namespace RecambiosWOW.Application.Services;

public record UpdateVehicleVariantDto
{
    public int Id { get; init; }
    public string ManufacturerCode { get; init; } = string.Empty;
    public EngineDetailsDto Engine { get; init; } = null!;
    public DimensionsDto Dimensions { get; init; } = null!;
    public VehicleDetailsDto Details { get; init; } = null!;
}