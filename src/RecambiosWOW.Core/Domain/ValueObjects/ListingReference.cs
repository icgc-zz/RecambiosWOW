using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Core.Domain.ValueObjects;

public record ListingReference
{
    public string Id { get; }
    public ListingType Type { get; }
    public string Source { get; }
    public Uri? ExternalUrl { get; }

    public ListingReference(
        string id,
        ListingType type,
        string source,
        Uri? externalUrl = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));
       
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("Source cannot be empty", nameof(source));

        if (type == ListingType.External && externalUrl == null)
            throw new ArgumentException("External URL is required for external listings");

        Id = id;
        Type = type;
        Source = source;
        ExternalUrl = externalUrl;
    }
}