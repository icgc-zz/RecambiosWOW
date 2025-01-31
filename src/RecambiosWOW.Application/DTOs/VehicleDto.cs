namespace RecambiosWOW.Application.DTOs;

public class VehicleDto
{
    public VehicleDto(VehicleIdentifierDto identifier, VehicleSpecificationDto? specifications)
    {
        Identifier = identifier;
    }

    public int Id { get; init; }
    public VehicleIdentifierDto Identifier { get; set; }
    public string? VIN { get; set; }
    public string? LicensePlate { get; set; }
    public VehicleSpecificationDto? Specifications { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}