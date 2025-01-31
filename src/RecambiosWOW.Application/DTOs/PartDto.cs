using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Application.DTOs;

public class PartDto
{
    public int Id { get; set; }
    public string SerialNumber { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public PartCondition Condition { get; set; }
    public decimal Price { get; set; }
    public PartSource Source { get; set; }
    public List<VehicleCompatibilityDto> Compatibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}