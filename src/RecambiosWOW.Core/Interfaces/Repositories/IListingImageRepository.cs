using RecambiosWOW.Core.Domain.Entities;

namespace RecambiosWOW.Core.Interfaces.Repositories;

public interface IListingImageRepository
{
    Task<ListingImage> GetByIdAsync(int id);
    Task<IEnumerable<ListingImage>> GetByListingIdAsync(int listingId);
    Task<ListingImage> AddAsync(ListingImage image);
    Task<ListingImage> UpdateAsync(ListingImage image);
    Task<bool> DeleteAsync(int id);
}