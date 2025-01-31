using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class VehicleModel
{
    public int Id { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    private readonly List<VehicleVariant> _variants;
    public IReadOnlyCollection<VehicleVariant> Variants => _variants.AsReadOnly();
    public AuditInfo AuditInfo { get; private set; }

    public VehicleModel(string make, string model, int year)
    {
        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("Make cannot be empty", nameof(make));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty", nameof(model));

        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Invalid year", nameof(year));

        Make = make;
        Model = model;
        Year = year;
        _variants = new List<VehicleVariant>();
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void AddVariant(VehicleVariant variant)
    {
        if (variant == null)
            throw new ArgumentNullException(nameof(variant));

        if (_variants.Any(v => v.ManufacturerCode == variant.ManufacturerCode))
            throw new InvalidOperationException($"Variant with code {variant.ManufacturerCode} already exists");

        _variants.Add(variant);
        AuditInfo = AuditInfo.Update();
    }

    public VehicleModel()
    {
        _variants = new List<VehicleVariant>();
    }
}

