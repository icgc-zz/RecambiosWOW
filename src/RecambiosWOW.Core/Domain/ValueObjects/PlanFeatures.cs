namespace RecambiosWOW.Core.Domain.ValueObjects;

public record PlanFeatures
{
    public int MaxWatchLists { get; }
    public int MaxSavedSearches { get; }
    public bool CanAccessExternalLinks { get; }
    public bool HasAdvancedSearch { get; }

    public PlanFeatures(
        int maxWatchLists,
        int maxSavedSearches,
        bool canAccessExternalLinks,
        bool hasAdvancedSearch)
    {
        if (maxWatchLists < 0)
            throw new ArgumentException("Max watch lists cannot be negative", nameof(maxWatchLists));
        if (maxSavedSearches < 0)
            throw new ArgumentException("Max saved searches cannot be negative", nameof(maxSavedSearches));

        MaxWatchLists = maxWatchLists;
        MaxSavedSearches = maxSavedSearches;
        CanAccessExternalLinks = canAccessExternalLinks;
        HasAdvancedSearch = hasAdvancedSearch;
    }
}