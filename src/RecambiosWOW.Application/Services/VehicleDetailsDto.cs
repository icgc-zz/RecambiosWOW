namespace RecambiosWOW.Application.Services;

public record VehicleDetailsDto
{
    public string BodyType { get; init; } = string.Empty;
    public int Doors { get; init; }
    public int Seats { get; init; }
    public string DriveType { get; init; } = string.Empty;
    public string TransmissionType { get; init; } = string.Empty;
}