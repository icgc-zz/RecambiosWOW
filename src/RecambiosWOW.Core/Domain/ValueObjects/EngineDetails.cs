namespace RecambiosWOW.Core.Domain.ValueObjects;

public record EngineDetails
{
    public string Type { get; }
    public int Displacement { get; }
    public int HorsePower { get; }
    public string FuelType { get; }
    public string EmissionStandard { get; }

    public EngineDetails(
        string type, 
        int displacement, 
        int horsePower, 
        string fuelType,
        string emissionStandard)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Engine type cannot be empty", nameof(type));
            
        if (displacement <= 0)
            throw new ArgumentException("Displacement must be positive", nameof(displacement));
            
        if (horsePower <= 0)
            throw new ArgumentException("Horse power must be positive", nameof(horsePower));

        Type = type;
        Displacement = displacement;
        HorsePower = horsePower;
        FuelType = fuelType ?? throw new ArgumentNullException(nameof(fuelType));
        EmissionStandard = emissionStandard ?? throw new ArgumentNullException(nameof(emissionStandard));
    }
    
}