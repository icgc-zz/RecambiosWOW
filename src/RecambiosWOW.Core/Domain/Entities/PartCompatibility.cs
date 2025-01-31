namespace RecambiosWOW.Core.Domain.Entities;

public class PartCompatibility
{
    public int Id { get; private set; }
    public Part Part { get; private set; }
    public VehicleModel VehicleModel { get; private set; }
    public VehicleVariant? VehicleVariant { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PartCompatibility(
        Part part,
        VehicleModel model,
        VehicleVariant? variant = null,
        string? notes = null)
    {
        Part = part ?? throw new ArgumentNullException(nameof(part));
        VehicleModel = model ?? throw new ArgumentNullException(nameof(model));
        
        if (variant != null && !model.Variants.Contains(variant))
            throw new InvalidOperationException("Variant must belong to the vehicle model");
            
        VehicleVariant = variant;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }

    protected PartCompatibility() { }
}