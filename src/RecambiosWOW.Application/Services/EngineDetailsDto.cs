namespace RecambiosWOW.Application.Services;

public record EngineDetailsDto
{
    public string Type { get; init; } = string.Empty;
    public int Displacement { get; init; }
    public int HorsePower { get; init; }
    public string FuelType { get; init; } = string.Empty;
    public string EmissionStandard { get; init; } = string.Empty;
}