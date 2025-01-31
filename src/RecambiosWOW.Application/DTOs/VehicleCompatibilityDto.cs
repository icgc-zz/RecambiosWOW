namespace RecambiosWOW.Application.DTOs;

public class VehicleCompatibilityDto
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public string EngineCode { get; set; }
    public string TransmissionCode { get; set; }
}