namespace RecambiosWOW.Application.DTOs;

public class VehicleSpecificationDto
{
    public string EngineCode { get; set; }
    public string TransmissionCode { get; set; }
    public EngineDetailsDto Engine { get; set; }
    public DimensionsDto Dimensions { get; set; }
    public VehicleDetailsDto Details { get; set; }
}