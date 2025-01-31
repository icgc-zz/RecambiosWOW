using RecambiosWOW.Core.Domain.Enums;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class SavedListing
{
    public int Id { get; private set; }
    public ListingReference Listing { get; private set; }
    public SavedListingStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime SavedAt { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    public SavedListing(
        ListingReference listing,
        string? notes = null)
    {
        Listing = listing ?? throw new ArgumentNullException(nameof(listing));
        Notes = notes;
        Status = SavedListingStatus.Active;
        SavedAt = DateTime.UtcNow;
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        AuditInfo = AuditInfo.Update();
    }

    public void UpdateStatus(SavedListingStatus status)
    {
        Status = status;
        AuditInfo = AuditInfo.Update();
    }

    protected SavedListing() { }
}
