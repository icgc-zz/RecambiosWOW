namespace RecambiosWOW.Infrastructure.Data.Models;

using SQLite;

[Table("Vehicles")]
public class VehicleModel()
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    // VehicleIdentifier properties
    [Indexed]
    public required string Make { get; set; }
    [Indexed]
    public required string Model { get; set; }
    [Indexed]
    public required int Year { get; set; }
 
    public string? VIN { get; set; }
   public string? LicensePlate { get; set; }


    // Store specifications as JSON
    public string? SpecificationsJson { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
