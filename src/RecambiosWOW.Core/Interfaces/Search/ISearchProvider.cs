using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.Models;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Core.Interfaces.Search;

public interface ISearchProvider
{
    Task<SearchResult<Part>> SearchPartsAsync(PartSearchCriteria criteria);
    Task IndexPartAsync(Part part);
    Task RemoveFromIndexAsync(int partId);
    Task RebuildIndexAsync();
}