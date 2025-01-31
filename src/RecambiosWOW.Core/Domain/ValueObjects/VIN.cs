namespace RecambiosWOW.Core.Domain.ValueObjects;

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
