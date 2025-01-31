namespace RecambiosWOW.Core.Domain.Entities;

public class ListingImage
{
    public int Id { get; private set; }
    public PartListing Listing { get; private set; }
    public string Url { get; private set; }
    public string? Caption { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsPrimary { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    public ListingImage(
        PartListing listing,
        string url,
        string? caption = null,
        int displayOrder = 0,
        bool isPrimary = false)
    {
        Listing = listing ?? throw new ArgumentNullException(nameof(listing));
        
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        Url = url;
        Caption = caption;
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void UpdateCaption(string? caption)
    {
        Caption = caption;
        AuditInfo = AuditInfo.Update();
    }

    protected ListingImage() { }
}
