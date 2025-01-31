using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Application.DTOs;

public record UpdateRegisteredVehicleDto
{
    public int Id { get; init; }
    public int VariantId { get; init; }
    public string? LicensePlate { get; init; }
    public Location Location { get; init; } = null!;
}