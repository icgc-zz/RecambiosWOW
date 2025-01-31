using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Infrastructure.Data.Models;

using SQLite;
using System.Text.Json;

// Infrastructure/Data/Models/PartModel.cs
[Table("Parts")]
public class PartModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    // PartIdentifier properties
    [Indexed]
    public string Manufacturer { get; set; }
    [Indexed]
    public string PartNumber { get; set; }
    public string SerialNumber { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public PartCondition Condition { get; set; }
    
    // Price properties
    public decimal PriceAmount { get; set; }
    public string PriceCurrency { get; set; }
    
    public PartSource Source { get; set; }
    
    // Store as JSON string
    public string CompatibilityJson { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}