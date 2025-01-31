using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.Models;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Core.Interfaces.Services;

public interface IPartSearchService
{
    Task<SearchResult<Part>> SearchAsync(
        PartSearchCriteria criteria, 
        CancellationToken cancellationToken = default);
}