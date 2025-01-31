using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class VehicleSpecification(
    string engineCode,
    string transmissionCode,
    VehicleDetails? details,
    EngineDetails? engine,
    Dimensions? dimensions)
{
    public string EngineCode { get; set; } = engineCode;
    public string TransmissionCode { get; set; } = transmissionCode;
    public VehicleDetails? Details { get; set; } = details;
    public EngineDetails? Engine { get; set; } = engine;
    public Dimensions? Dimensions { get; set; } = dimensions;
}
