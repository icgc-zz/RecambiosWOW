using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Core.Domain.ValueObjects;

public record ShippingOption
{
    public string Name { get; }
    public Price BasePrice { get; }
    public ShippingMethod Method { get; }
    public string? Carrier { get; }
    public int? EstimatedDays { get; }
    public bool IsActive { get; }
    public List<string> ExcludedCountries { get; }

    public ShippingOption(
        string name,
        Price basePrice,
        ShippingMethod method,
        string? carrier = null,
        int? estimatedDays = null,
        bool isActive = true,
        List<string>? excludedCountries = null)
    {
        // ... existing validation
        Name = name;
        BasePrice = basePrice;
        Method = method;
        Carrier = carrier;
        EstimatedDays = estimatedDays;
        IsActive = isActive;
        ExcludedCountries = excludedCountries ?? new List<string>();
    }
}

