namespace RecambiosWOW.Application.DTOs;

public record UpdateVehicleModelDto
{
    public int Id { get; init; }
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public int Year { get; init; }
}