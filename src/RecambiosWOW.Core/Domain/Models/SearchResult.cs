namespace RecambiosWOW.Core.Domain.Models;

public class SearchResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public bool HasNextPage { get; }

    public SearchResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        HasNextPage = (page * pageSize) < totalCount;
    }
}