using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Core.Interfaces.Marketplace;

public interface IMarketplaceProvider
{
    Task<IEnumerable<PartListing>> SearchPartsAsync(PartSearchCriteria criteria);
    Task<OrderResult> PlaceOrderAsync(OrderRequest request);
    Task<InventoryStatus> CheckInventoryAsync(string partId);
}