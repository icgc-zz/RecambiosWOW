namespace RecambiosWOW.Core.Domain.Entities;

public record PartIdentifier
{
    public string Manufacturer { get; }
    public string PartNumber { get; }
    public string? SerialNumber { get; }

    public PartIdentifier(string manufacturer, string partNumber, string? serialNumber = null)
    {
        if (string.IsNullOrWhiteSpace(manufacturer))
            throw new ArgumentException("Manufacturer cannot be empty", nameof(manufacturer));
            
        if (string.IsNullOrWhiteSpace(partNumber))
            throw new ArgumentException("Part number cannot be empty", nameof(partNumber));

        Manufacturer = manufacturer;
        PartNumber = partNumber;
        SerialNumber = serialNumber;
    }
}