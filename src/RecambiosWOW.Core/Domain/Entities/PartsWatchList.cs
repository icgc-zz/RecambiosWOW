using RecambiosWOW.Core.Domain.ValueObjects;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Core.Domain.Entities;

public class PartsWatchList
{
    public int Id { get; private set; }
    public Member Owner { get; private set; }
    public string Name { get; private set; }
    public PartSearchCriteria SearchCriteria { get; private set; }
    private readonly List<SavedListing> _savedListings;
    public IReadOnlyCollection<SavedListing> SavedListings => _savedListings.AsReadOnly();
    public bool NotificationsEnabled { get; private set; }
    public DateTime LastSearched { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    public PartsWatchList(
        Member owner,
        string name,
        PartSearchCriteria searchCriteria,
        bool notificationsEnabled = true)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        SearchCriteria = searchCriteria ?? throw new ArgumentNullException(nameof(searchCriteria));
        NotificationsEnabled = notificationsEnabled;
        _savedListings = new List<SavedListing>();
        LastSearched = DateTime.UtcNow;
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void UpdateSearchCriteria(PartSearchCriteria criteria)
    {
        SearchCriteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
        LastSearched = DateTime.UtcNow;
        AuditInfo = AuditInfo.Update();
    }

    public void ToggleNotifications(bool enabled)
    {
        NotificationsEnabled = enabled;
        AuditInfo = AuditInfo.Update();
    }

    public void AddSavedListing(ListingReference listing, string? notes = null)
    {
        if (_savedListings.Any(sl => sl.Listing.Equals(listing)))
            throw new InvalidOperationException("Listing is already saved");

        _savedListings.Add(new SavedListing(listing, notes));
        AuditInfo = AuditInfo.Update();
    }

    protected PartsWatchList()
    {
        _savedListings = new List<SavedListing>();
    }
}