using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Domain.Models;

public class VehicleInfo
{
    public int Id { get; }
    public string VIN { get; }
    public string Make { get; }
    public string Model { get; }
    public int Year { get; }
    public VehicleSpecification Specifications { get; }
    public string LicensePlate { get; }
    public string CountryOfRegistration { get; }
    public DateTime? RegistrationDate { get; }

    public VehicleInfo(
        string vin,
        string make,
        string model,
        int year,
        VehicleSpecification specifications,
        string licensePlate = null,
        string countryOfRegistration = null,
        DateTime? registrationDate = null)
    {
        VIN = vin ?? throw new ArgumentNullException(nameof(vin));
        Make = make ?? throw new ArgumentNullException(nameof(make));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Year = year;
        Specifications = specifications ?? throw new ArgumentNullException(nameof(specifications));
        LicensePlate = licensePlate;
        CountryOfRegistration = countryOfRegistration;
        RegistrationDate = registrationDate;
    }
}