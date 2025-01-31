using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Core.Domain.ValueObjects;

public class ActivityHistory
{
    private readonly List<ActivityEntry> _activities;
    public IReadOnlyCollection<ActivityEntry> Activities => _activities.AsReadOnly();

    public ActivityHistory()
    {
        _activities = new List<ActivityEntry>();
    }

    public void AddActivity(ActivityType type, string description, IDictionary<string, string>? metadata = null)
    {
        _activities.Add(new ActivityEntry(type, description, metadata));
    }
}