namespace RecambiosWOW.Core.Domain.ValueObjects;

public record VehicleIdentifier
{
    public string Make { get; }
    public string Model { get; }
    public int Year { get; }

    public VehicleIdentifier(string make, string model, int year)
    {
        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("make cannot be empty", nameof(make));
            
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("model number cannot be empty", nameof(model));

        Make = make;
        Model = model;
        Year = year;
    }

    public override string ToString() => 
        $"{Make}-{Model}{(Year != null ? $"-{Year}" : "")}";
}