using RecambiosWOW.Core.Domain.Enums;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class PartListing
{
    public int Id { get; private set; }
    public Member Seller { get; private set; }
    public InventoryItem InventoryItem { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Price Price { get; private set; }
    public Location Location { get; private set; }
    public int? MaxShippingDistance { get; private set; }  // in kilometers, null for unlimited
    public ShippingType ShippingType { get; private set; }
    public bool LocalPickupOnly { get; private set; }
    private readonly List<ShippingOption> _shippingOptions;
    public IReadOnlyCollection<ShippingOption> ShippingOptions => _shippingOptions.AsReadOnly();
    public decimal Weight { get; private set; }  // in kilograms
    public Dimensions PackageDimensions { get; private set; }
    public bool LocalPickupAllowed { get; private set; }
    public ListingStatus Status { get; private set; }
    private readonly List<ListingImage> _images;
    public IReadOnlyCollection<ListingImage> Images => _images.AsReadOnly();
    private readonly List<Question> _questions;
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
    private readonly List<PriceHistory> _priceHistory;
    public IReadOnlyCollection<PriceHistory> PriceHistory => _priceHistory.AsReadOnly();
    public ListingMetrics Metrics { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    public PartListing(
        Member seller,
        InventoryItem inventoryItem,
        string title,
        string description,
        Price price,
        Location location,
        decimal weight,
        Dimensions packageDimensions,
        ShippingType shippingType,
        int? maxShippingDistance = null
        )
        
    {
        Seller = seller ?? throw new ArgumentNullException(nameof(seller));
        InventoryItem = inventoryItem ?? throw new ArgumentNullException(nameof(inventoryItem));
        
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (weight < 0)
            throw new ArgumentException("Weight must be non-negative", nameof(weight));

        Location = location ?? throw new ArgumentNullException(nameof(location));
        MaxShippingDistance = maxShippingDistance;
        ShippingType = shippingType;
        Weight = weight;
        PackageDimensions = packageDimensions ?? throw new ArgumentNullException(nameof(packageDimensions));
        _shippingOptions = new List<ShippingOption>();

        // If LocalPickupOnly, we don't allow adding other shipping options
        if (shippingType == ShippingType.LocalPickupOnly)
        {
            _shippingOptions.Add(new ShippingOption(
                "Local Pickup",
                new Price(0, price.Currency),
                ShippingMethod.LocalPickup));
        }

        Title = title;
        Description = description;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Status = ListingStatus.Active;
        _images = new List<ListingImage>();
        _questions = new List<Question>();
        _priceHistory = new List<PriceHistory>
        {
            new(price, "Initial listing price")
        };
        Metrics = new ListingMetrics();
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void UpdatePrice(Price newPrice, string reason)
    {
        if (Status != ListingStatus.Active)
            throw new InvalidOperationException("Cannot update price of inactive listing");

        Price = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
        _priceHistory.Add(new PriceHistory(newPrice, reason));
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateLocation(Location location)
    {
        Location = location ?? throw new ArgumentNullException(nameof(location));
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateShippingOptions(bool localPickupOnly, int? maxShippingDistance)
    {
        LocalPickupOnly = localPickupOnly;
        MaxShippingDistance = maxShippingDistance;
        AuditInfo = AuditInfo.Update();
    }

    public bool IsWithinShippingRange(Location buyerLocation)
    {
        if (!MaxShippingDistance.HasValue)
            return true;

        if (Location.Coordinates == null || buyerLocation.Coordinates == null)
            return true;  // If we can't calculate distance, assume it's in range

        var distance = Location.Coordinates.CalculateDistanceTo(buyerLocation.Coordinates);
        return distance <= MaxShippingDistance.Value;
    }
    
    public void AddShippingOption(ShippingOption option)
    {
        if (ShippingType == ShippingType.LocalPickupOnly)
            throw new InvalidOperationException("Cannot add shipping options to local pickup only listings");

        if (_shippingOptions.Any(o => o.Name == option.Name))
            throw new InvalidOperationException($"Shipping option '{option.Name}' already exists");

        _shippingOptions.Add(option);
        AuditInfo = AuditInfo.Update();
    }

    public void RemoveShippingOption(string name)
    {
        var option = _shippingOptions.FirstOrDefault(o => o.Name == name);
        if (option != null)
        {
            _shippingOptions.Remove(option);
            AuditInfo = AuditInfo.Update();
        }
    }

    public Price CalculateShippingCost(ShippingOption option, Location destination)
    {
        if (!_shippingOptions.Contains(option))
            throw new ArgumentException("Invalid shipping option for this listing");

        if (option.ExcludedCountries.Contains(destination.Country))
            throw new InvalidOperationException("Shipping option not available for destination country");

        return option.Method switch
        {
            ShippingMethod.FlatRate => option.BasePrice,
            ShippingMethod.DistanceBased => CalculateDistanceBasedShipping(option, destination),
            ShippingMethod.WeightBased => CalculateWeightBasedShipping(option),
            ShippingMethod.CarrierCalculated => CalculateCarrierShipping(option, destination),
            ShippingMethod.LocalPickup => new Price(0, option.BasePrice.Currency),
            _ => throw new NotImplementedException()
        };
    }

    private Price CalculateDistanceBasedShipping(ShippingOption option, Location destination)
    {
        if (Location.Coordinates == null || destination.Coordinates == null)
            return option.BasePrice;

        var distance = Location.Coordinates.CalculateDistanceTo(destination.Coordinates);
        var ratePerKm = 0.1m;  // This could be configurable
        var distanceCost = (decimal)distance * ratePerKm;
        
        return new Price(
            option.BasePrice.Amount + distanceCost,
            option.BasePrice.Currency);
    }

    private Price CalculateWeightBasedShipping(ShippingOption option)
    {
        var ratePerKg = 2.0m;  // This could be configurable
        var weightCost = Weight * ratePerKg;
        
        return new Price(
            option.BasePrice.Amount + weightCost,
            option.BasePrice.Currency);
    }

    private Price CalculateCarrierShipping(ShippingOption option, Location destination)
    {
        // This would integrate with carrier APIs
        throw new NotImplementedException("Carrier shipping calculation not implemented");
    }
    protected PartListing()
    {
        _images = new List<ListingImage>();
        _questions = new List<Question>();
        _priceHistory = new List<PriceHistory>();
    }
}