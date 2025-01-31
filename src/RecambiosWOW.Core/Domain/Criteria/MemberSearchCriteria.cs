using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Core.Domain.Criteria;

public class MemberSearchCriteria
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public MembershipType? Type { get; set; }
    public bool? IsActive { get; set; }
}