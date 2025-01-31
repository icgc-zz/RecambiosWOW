using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Core.Domain.Entities;

public class InventoryTransaction
{
    public int Id { get; private set; }
    public TransactionType Type { get; private set; }
    public int Quantity { get; private set; }
    public string Reference { get; private set; }
    public string? Notes { get; private set; }
    public DateTime OccurredAt { get; private set; }

    public InventoryTransaction(
        TransactionType type,
        int quantity,
        string reference,
        string? notes = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));
       
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Reference cannot be empty", nameof(reference));

        Type = type;
        Quantity = quantity;
        Reference = reference;
        Notes = notes;
        OccurredAt = DateTime.UtcNow;
    }

    protected InventoryTransaction() { }
}