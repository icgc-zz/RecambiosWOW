namespace RecambiosWOW.Core.Domain.ValueObjects;

public record VehicleDetails
{
    public string BodyType { get; }
    public int Doors { get; }
    public int Seats { get; }
    public string DriveType { get; }
    public string TransmissionType { get; }

    public VehicleDetails(
        string bodyType,
        int doors,
        int seats,
        string driveType,
        string transmissionType)
    {
        if (string.IsNullOrWhiteSpace(bodyType))
            throw new ArgumentException("Body type cannot be empty", nameof(bodyType));
            
        if (doors <= 0)
            throw new ArgumentException("Doors must be positive", nameof(doors));
            
        if (seats <= 0)
            throw new ArgumentException("Seats must be positive", nameof(seats));

        BodyType = bodyType;
        Doors = doors;
        Seats = seats;
        DriveType = driveType ?? throw new ArgumentNullException(nameof(driveType));
        TransmissionType = transmissionType ?? throw new ArgumentNullException(nameof(transmissionType));
    }
}