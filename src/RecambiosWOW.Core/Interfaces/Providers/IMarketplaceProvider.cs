using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.Enums;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Core.Interfaces.Providers;

public interface IMarketplaceProvider
{
    Task<IEnumerable<PartListing>> SearchPartsAsync(PartSearchCriteria criteria);
    Task<OrderResult> PlaceOrderAsync(OrderRequest request);
    Task<InventoryStatus> CheckInventoryAsync(string partId);
}