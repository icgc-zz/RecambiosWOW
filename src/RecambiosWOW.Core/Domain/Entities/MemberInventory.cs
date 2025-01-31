using RecambiosWOW.Core.Domain.Enums;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class MemberInventory
{
    public int Id { get; private set; }
    public Member Owner { get; private set; }
    public string Name { get; private set; }
    private readonly List<InventoryItem> _items;
    public IReadOnlyCollection<InventoryItem> Items => _items.AsReadOnly();
    public InventoryStatus Status { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    public MemberInventory(Member owner, string name)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        _items = new List<InventoryItem>();
        Status = InventoryStatus.Active;
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void AddItem(Part part, PartCondition condition, int quantity, Price listingPrice, string? location = null)
    {
        var item = new InventoryItem(part, condition, quantity, listingPrice, location);
        _items.Add(item);
        AuditInfo = AuditInfo.Update();
    }

    protected MemberInventory()
    {
        _items = new List<InventoryItem>();
    }
}
