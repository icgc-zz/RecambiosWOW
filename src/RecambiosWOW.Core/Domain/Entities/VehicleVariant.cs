using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

// Core/Domain/Entities/VehicleVariant.cs
public class VehicleVariant
{
    public int Id { get; private set; }
    public VehicleModel Model { get; private set; }
    public string ManufacturerCode { get; private set; }
    public EngineDetails Engine { get; private set; }
    public Dimensions Dimensions { get; private set; }
    public VehicleDetails Details { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    public VehicleVariant(
        VehicleModel model,
        string manufacturerCode,
        EngineDetails engine,
        Dimensions dimensions,
        VehicleDetails details)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        
        if (string.IsNullOrWhiteSpace(manufacturerCode))
            throw new ArgumentException("Manufacturer code cannot be empty", nameof(manufacturerCode));
            
        ManufacturerCode = manufacturerCode;
        Engine = engine ?? throw new ArgumentNullException(nameof(engine));
        Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
        Details = details ?? throw new ArgumentNullException(nameof(details));
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void UpdateEngine(EngineDetails engine)
    {
        Engine = engine ?? throw new ArgumentNullException(nameof(engine));
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateDimensions(Dimensions dimensions)
    {
        Dimensions = dimensions ?? throw new ArgumentNullException(nameof(dimensions));
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateDetails(VehicleDetails details)
    {
        Details = details ?? throw new ArgumentNullException(nameof(details));
        AuditInfo = AuditInfo.Update();
    }

    public VehicleVariant() { }
}