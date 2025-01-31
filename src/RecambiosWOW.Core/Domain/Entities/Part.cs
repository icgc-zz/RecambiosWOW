using RecambiosWOW.Core.Domain.Enums;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class Part
{
    public int Id { get; private set; }
    public PartIdentifier Identifier { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public PartCondition Condition { get; private set; }
    public Price Price { get; private set; }
    public PartSource Source { get; private set; }
    public IReadOnlyCollection<VehicleCompatibility> Compatibility => _compatibility.AsReadOnly();
    private List<VehicleCompatibility> _compatibility;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Constructor for new parts
    public Part(
        PartIdentifier identifier,
        string name,
        string description,
        PartCondition condition,
        Price price,
        PartSource source)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        UpdateDetails(name, description, condition, price);
        Source = source;
        _compatibility = new List<VehicleCompatibility>();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        string description,
        PartCondition condition,
        Price price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        Condition = condition;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCompatibility(VehicleCompatibility compatibility)
    {
        if (compatibility == null)
            throw new ArgumentNullException(nameof(compatibility));

        _compatibility.Add(compatibility);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveCompatibility(VehicleCompatibility compatibility)
    {
        _compatibility.Remove(compatibility);
        UpdatedAt = DateTime.UtcNow;
    }

    // Protected constructor for ORM
    public Part() { }
}
