using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Core.Domain.ValueObjects;

public record ActivityEntry
{
    public ActivityType Type { get; }
    public string Description { get; }
    public DateTime Timestamp { get; }
    public IReadOnlyDictionary<string, string>? Metadata { get; }

    public ActivityEntry(
        ActivityType type,
        string description,
        IDictionary<string, string>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Type = type;
        Description = description;
        Timestamp = DateTime.UtcNow;
        Metadata = metadata?.AsReadOnly();
    }
}