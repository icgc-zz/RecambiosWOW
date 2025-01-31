using RecambiosWOW.Core.Domain.Enums;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class InventoryItem
{
    public int Id { get; private set; }
    public Part Part { get; private set; }
    public PartCondition Condition { get; private set; }
    public int Quantity { get; private set; }
    public Price ListingPrice { get; private set; }
    public string? Location { get; private set; }
    public PartListing? ActiveListing { get; private set; }
    private readonly List<InventoryTransaction> _transactions;
    public IReadOnlyCollection<InventoryTransaction> Transactions => _transactions.AsReadOnly();
    public AuditInfo AuditInfo { get; private set; }

    public InventoryItem(
        Part part,
        PartCondition condition,
        int quantity,
        Price listingPrice,
        string? location = null)
    {
        Part = part ?? throw new ArgumentNullException(nameof(part));
        Condition = condition;  // Direct assignment
        
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

        Quantity = quantity;
        ListingPrice = listingPrice ?? throw new ArgumentNullException(nameof(listingPrice));
        Location = location;
        _transactions = new List<InventoryTransaction>();
        AuditInfo = new AuditInfo(DateTime.UtcNow);

        RecordTransaction(TransactionType.Initial, quantity, "Initial stock");
    }

    public void UpdateQuantity(int newQuantity, string reason)
    {
        if (newQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(newQuantity));

        var difference = newQuantity - Quantity;
        Quantity = newQuantity;
        
        RecordTransaction(
            difference > 0 ? TransactionType.Addition : TransactionType.Reduction,
            Math.Abs(difference),
            reason);

        AuditInfo = AuditInfo.Update();
    }

    private void RecordTransaction(TransactionType type, int quantity, string reason)
    {
        _transactions.Add(new InventoryTransaction(type, quantity, reason));
    }

    protected InventoryItem()
    {
        _transactions = new List<InventoryTransaction>();
    }
}