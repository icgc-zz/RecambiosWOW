using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class PriceHistory
{
    public int Id { get; private set; }
    public Price Price { get; private set; }
    public string Reason { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PriceHistory(Price price, string reason)
    {
        Price = price ?? throw new ArgumentNullException(nameof(price));
        
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason cannot be empty", nameof(reason));

        Reason = reason;
        CreatedAt = DateTime.UtcNow;
    }

    protected PriceHistory() { }
}