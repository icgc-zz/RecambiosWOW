using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class Vehicle
{
    public int Id { get; private set; }
    public VehicleIdentifier Identifier { get; private set; }
    public VIN? VIN { get; private set; }
    public LicensePlate? LicensePlate { get; private set; }
    public VehicleSpecification Specifications { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    // Constructor for new vehicles
    public Vehicle(
        VehicleIdentifier identifier,
        VIN? vin,
        LicensePlate? licensePlate,
        VehicleSpecification specifications)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        VIN = vin;
        LicensePlate = licensePlate;
        Specifications = specifications ?? throw new ArgumentNullException(nameof(specifications));
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void UpdateSpecifications(VehicleSpecification specifications)
    {
        Specifications = specifications ?? throw new ArgumentNullException(nameof(specifications));
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateIdentifier(VehicleIdentifier identifier)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateVIN(VIN? vin)
    {
        VIN = vin;
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateLicensePlate(LicensePlate? licensePlate)
    {
        LicensePlate = licensePlate;
        AuditInfo = AuditInfo.Update();
    }

    // Protected constructor for ORM
    protected Vehicle() { }
}

public record VehicleIdentifier
{
    public string Make { get; }
    public string Model { get; }
    public int Year { get; }

    public VehicleIdentifier(string make, string model, int year)
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
    }
}

public record VIN
{
    public string Value { get; }

    public VIN(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("VIN cannot be empty", nameof(value));

        if (value.Length != 17)
            throw new ArgumentException("VIN must be 17 characters long", nameof(value));

        Value = value.ToUpperInvariant();
    }
}

public record LicensePlate
{
    public string Value { get; }

    public LicensePlate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("License plate cannot be empty", nameof(value));

        Value = value.ToUpperInvariant();
    }
}

public record AuditInfo
{
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    public AuditInfo(DateTime createdAt, DateTime? updatedAt = null)
    {
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public AuditInfo Update() => new(CreatedAt, DateTime.UtcNow);
}