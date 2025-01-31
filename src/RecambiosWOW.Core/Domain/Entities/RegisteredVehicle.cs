using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class RegisteredVehicle
{
    public int Id { get; private set; }
    public Member Owner { get; private set; }
    public VehicleModel Model { get; private set; }
    public VehicleVariant Variant { get; private set; }
    public VIN VIN { get; private set; }
    public Location Location { get; private set; }
    public LicensePlate? LicensePlate { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    public RegisteredVehicle(
        Member owner,
        VehicleModel model,
        VehicleVariant variant,
        VIN vin,
        Location location,
        LicensePlate? licensePlate = null)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        
        if (!model.Variants.Contains(variant))
            throw new InvalidOperationException("Variant must belong to the vehicle model");
            
        Variant = variant ?? throw new ArgumentNullException(nameof(variant));
        VIN = vin ?? throw new ArgumentNullException(nameof(vin));
        LicensePlate = licensePlate;
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void UpdateLicensePlate(LicensePlate? licensePlate)
    {
        LicensePlate = licensePlate;
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateVariant(VehicleVariant variant)
    {
        if (!Model.Variants.Contains(variant))
            throw new InvalidOperationException("Variant must belong to the vehicle model");

        Variant = variant ?? throw new ArgumentNullException(nameof(variant));
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateLocation(Location location)
    {
        Location = location ?? throw new ArgumentNullException(nameof(location));
        AuditInfo = AuditInfo.Update();
    }

    protected RegisteredVehicle() { }
}