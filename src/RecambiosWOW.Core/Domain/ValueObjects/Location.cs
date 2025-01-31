namespace RecambiosWOW.Core.Domain.ValueObjects;

// Core/Domain/ValueObjects/Location.cs
public record Location
{
    public string Country { get; }
    public string? Region { get; }  // State/Province/Region
    public string City { get; }
    public string PostalCode { get; }
    public GeoCoordinates? Coordinates { get; }  // Optional for precise location

    public Location(
        string country,
        string city,
        string postalCode,
        string? region = null,
        GeoCoordinates? coordinates = null)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));

        Country = country;
        City = city;
        PostalCode = postalCode;
        Region = region;
        Coordinates = coordinates;
    }
}

public record GeoCoordinates
{
    public double Latitude { get; }
    public double Longitude { get; }

    public GeoCoordinates(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
    }

    public double CalculateDistanceTo(GeoCoordinates other)
    {
        // Haversine formula implementation for distance calculation
        // Returns distance in kilometers
        // Implementation here...
        return 0;
    }
}