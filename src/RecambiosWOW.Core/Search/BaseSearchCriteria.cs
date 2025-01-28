namespace RecambiosWOW.Core.Criteria;

public abstract class BaseSearchCriteria
{
    public int Skip { get; private set; }
    public int Take { get; private set; }
    public string SortBy { get; private set; }
    public bool SortDescending { get; private set; }

    protected BaseSearchCriteria(
        int skip = 0,
        int take = 20,
        string sortBy = null,
        bool sortDescending = false)
    {
        Skip = Math.Max(0, skip);
        Take = Math.Clamp(take, 1, 100);
        SortBy = sortBy;
        SortDescending = sortDescending;
    }
}