using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Application.DTOs;

public record CreateRegisteredVehicleDto
{
    public int OwnerId { get; init; }
    public int ModelId { get; init; }
    public int VariantId { get; init; }
    public string VIN { get; init; } = string.Empty;
    public string? LicensePlate { get; init; }
    public Location Location { get; init; } = null!;
}