using RecambiosWOW.Application.Services;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Application.DTOs;

public record RegisteredVehicleDto
{
    public int Id { get; init; }
    public int OwnerId { get; init; }
    public VehicleModelDto Model { get; init; } = null!;
    public VehicleVariantDto Variant { get; init; } = null!;
    public string VIN { get; init; } = string.Empty;
    public string? LicensePlate { get; init; }
    public Location Location { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}