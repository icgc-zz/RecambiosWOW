namespace RecambiosWOW.Core.Criteria;

public class VehicleSearchCriteria : BaseSearchCriteria
{
    public string SearchTerm { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public int? Year { get; private set; }
    public string EngineCode { get; private set; }
    public string FuelType { get; private set; }
    public string TransmissionType { get; private set; }
    public string BodyType { get; private set; }
    public string LicensePlate { get; private set; }
    public string VIN { get; private set; }

    public VehicleSearchCriteria(
        string searchTerm = null,
        string make = null,
        string model = null,
        int? year = null,
        string engineCode = null,
        string fuelType = null,
        string transmissionType = null,
        string bodyType = null,
        string licensePlate = null,
        string vin = null,
        int skip = 0,
        int take = 20,
        string sortBy = null,
        bool sortDescending = false)
        : base(skip, take, sortBy, sortDescending)
    {
        SearchTerm = searchTerm?.Trim();
        Make = make?.Trim();
        Model = model?.Trim();
        Year = year;
        EngineCode = engineCode?.Trim().ToUpperInvariant();
        FuelType = fuelType?.Trim();
        TransmissionType = transmissionType?.Trim();
        BodyType = bodyType?.Trim();
        LicensePlate = licensePlate?.Trim().ToUpperInvariant();
        VIN = vin?.Trim().ToUpperInvariant();

        ValidateYear();
        ValidateVIN();
    }

    private void ValidateYear()
    {
        if (Year.HasValue)
        {
            if (Year.Value < 1900 || Year.Value > DateTime.UtcNow.Year + 1)
            {
                throw new ArgumentException("Year must be between 1900 and next year", nameof(Year));
            }
        }
    }

    private void ValidateVIN()
    {
        if (!string.IsNullOrEmpty(VIN) && VIN.Length != 17)
        {
            throw new ArgumentException("VIN must be 17 characters long", nameof(VIN));
        }
    }
}