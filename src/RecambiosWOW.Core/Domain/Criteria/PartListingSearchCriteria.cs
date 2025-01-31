using RecambiosWOW.Core.Domain.Enums;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Criteria;

public class PartListingSearchCriteria
{
    public int? SellerId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public Location? Location { get; set; }
    public int? MaxDistance { get; set; }
    public ShippingType? ShippingType { get; set; }
    public ListingStatus? Status { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}