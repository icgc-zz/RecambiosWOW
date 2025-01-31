using RecambiosWOW.Core.Domain.Criteria;
using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IPartListingRepository
{
    Task<PartListing> GetByIdAsync(int id);
    Task<IEnumerable<PartListing>> GetBySellerIdAsync(int sellerId);
    Task<IEnumerable<PartListing>> SearchAsync(PartListingSearchCriteria criteria);
    Task<int> GetTotalCountAsync(PartListingSearchCriteria criteria);
    Task<PartListing> AddAsync(PartListing listing);
    Task<PartListing> UpdateAsync(PartListing listing);
    Task<bool> DeleteAsync(int id);
}