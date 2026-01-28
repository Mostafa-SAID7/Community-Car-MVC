namespace CommunityCar.Domain.Policies.Account.Core;

public class MfaRequiredResource
{
    public string ResourceName { get; set; } = string.Empty;
    public bool RequiresMfa { get; set; }
    public bool RequiresFreshMfa { get; set; }
    public MfaRequirement RequirementLevel { get; set; } = MfaRequirement.Optional;
}