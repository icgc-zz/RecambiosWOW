namespace RecambiosWOW.Core.Search;

public abstract class BaseSearchCriteria
{
    public int Skip { get; set; }
    public int Take { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }

    protected BaseSearchCriteria(
        int skip = 0,
        int take = 20,
        string? sortBy = null,
        bool sortDescending = false)
    {
        Skip = Math.Max(0, skip);
        Take = Math.Clamp(take, 1, 100);
        SortBy = sortBy;
        SortDescending = sortDescending;
    }
}