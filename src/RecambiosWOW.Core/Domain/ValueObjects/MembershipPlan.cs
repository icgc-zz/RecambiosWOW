namespace RecambiosWOW.Core.Domain.ValueObjects;

public record MembershipPlan
{
    public string Name { get; }
    public PlanFeatures Features { get; }
    public DateTime ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public MembershipPlan(string name, PlanFeatures features, DateTime validFrom, DateTime? validTo = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        Features = features ?? throw new ArgumentNullException(nameof(features));
        ValidFrom = validFrom;
        ValidTo = validTo;

        if (validTo.HasValue && validTo.Value <= validFrom)
            throw new ArgumentException("Valid to date must be after valid from date");
    }

    public bool IsValid(DateTime date) => 
        date >= ValidFrom && (!ValidTo.HasValue || date <= ValidTo.Value);
}