namespace RecambiosWOW.Core.Domain.ValueObjects;

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
