namespace RecambiosWOW.Core.Interfaces.Repositories;

/// <summary>
/// Interface for paginated results
/// </summary>
public interface IPagedResult<T>
{
    int PageNumber { get; }
    int PageSize { get; }
    int TotalPages { get; }
    int TotalCount { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
    IReadOnlyList<T> Items { get; }
}