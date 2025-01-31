namespace RecambiosWOW.Application.DTOs;

public record CreateVehicleModelDto
{
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public int Year { get; init; }
}