namespace RecambiosWOW.Core.Domain.ValueObjects;

public record VehicleCompatibility
{
    public string Make { get; }
    public string Model { get; }
    public YearRange YearRange { get; }
    public string EngineCode { get; }
    public string TransmissionCode { get; }

    public VehicleCompatibility(
        string make, 
        string model, 
        YearRange yearRange, 
        string engineCode = null, 
        string transmissionCode = null)
    {
        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("Make cannot be empty", nameof(make));
            
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty", nameof(model));

        Make = make;
        Model = model;
        YearRange = yearRange ?? throw new ArgumentNullException(nameof(yearRange));
        EngineCode = engineCode;
        TransmissionCode = transmissionCode;
    }
}