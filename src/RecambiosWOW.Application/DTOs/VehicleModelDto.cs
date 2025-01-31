using RecambiosWOW.Application.Services;

namespace RecambiosWOW.Application.DTOs;

public record VehicleModelDto
{
    public int Id { get; init; }
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public int Year { get; init; }
    public IReadOnlyCollection<VehicleVariantDto> Variants { get; init; } = 
        Array.Empty<VehicleVariantDto>();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}