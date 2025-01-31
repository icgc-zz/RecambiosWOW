
namespace RecambiosWOW.Core.Domain.Criteria;

public class VehicleModelSearchCriteria
{
    public string? SearchTerm { get; init; }
    public string? Make { get; init; }
    public string? Model { get; init; }
    public int? Year { get; init; }
    public int? YearFrom { get; init; }
    public int? YearTo { get; init; }
    public bool IncludeVariants { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
    public int Skip { get; init; }
    public int Take { get; init; }

    public VehicleModelSearchCriteria(
        string? searchTerm = null,
        string? make = null,
        string? model = null,
        int? year = null,
        int? yearFrom = null,
        int? yearTo = null,
        bool includeVariants = false,
        string? sortBy = null,
        bool sortDescending = false,
        int skip = 0,
        int take = 20)
    {
        SearchTerm = searchTerm;
        Make = make;
        Model = model;
        Year = year;
        YearFrom = yearFrom;
        YearTo = yearTo;
        IncludeVariants = includeVariants;
        SortBy = sortBy;
        SortDescending = sortDescending;
        Skip = Math.Max(0, skip);
        Take = Math.Clamp(take, 1, 100);

        Validate();
    }

    private void Validate()
    {
        if (Year.HasValue && (YearFrom.HasValue || YearTo.HasValue))
            throw new ArgumentException("Cannot specify both Year and YearFrom/YearTo");

        if (YearFrom.HasValue && YearTo.HasValue && YearFrom > YearTo)
            throw new ArgumentException("YearFrom must be less than or equal to YearTo");

        var currentYear = DateTime.UtcNow.Year;
        var validYearRange = new Range(1900, currentYear + 1);
        const int minYear = 1900;
        var maxYear = DateTime.UtcNow.Year + 1;
        
        if (Year.HasValue && (Year.Value < minYear || Year.Value > maxYear))
            throw new ArgumentException($"Year must be between {minYear} and {maxYear}");
        
        if (YearFrom is < minYear)
            throw new ArgumentException($"Year must be between {minYear} and {maxYear}");

        if (YearTo > maxYear)
            throw new ArgumentException($"Year must be between {minYear} and {maxYear}");
    }
}
