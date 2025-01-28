using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Core.Criteria;

public class PartSearchCriteria : BaseSearchCriteria
{
    public string SearchTerm { get; private set; }
    public PartCondition? Condition { get; private set; }
    public string Manufacturer { get; private set; }
    public decimal? MaxPrice { get; private set; }
    public decimal? MinPrice { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public int? Year { get; private set; }

    public PartSearchCriteria(
        string searchTerm = null,
        PartCondition? condition = null,
        string manufacturer = null,
        decimal? maxPrice = null,
        decimal? minPrice = null,
        string make = null,
        string model = null,
        int? year = null,
        int skip = 0,
        int take = 20,
        string sortBy = null,
        bool sortDescending = false)
        : base(skip, take, sortBy, sortDescending)
    {
        SearchTerm = searchTerm?.Trim();
        Condition = condition;
        Manufacturer = manufacturer?.Trim();
        MaxPrice = maxPrice;
        MinPrice = minPrice;
        Make = make?.Trim();
        Model = model?.Trim();
        Year = year;

        ValidatePriceRange();
    }

    private void ValidatePriceRange()
    {
        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice.Value > MaxPrice.Value)
        {
            throw new ArgumentException("Minimum price cannot be greater than maximum price");
        }
    }
}