using CommunityCar.Application.Common.Models.Identity;

namespace CommunityCar.Web.Models.Account.Management;

public class AccountInfoVM : UserSummaryVM
{
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsDeactivated { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public string? DeactivationReason { get; set; }
    public int ActiveSessions { get; set; }
    public bool HasLinkedAccounts { get; set; }
    public new DateTime CreatedAt { get; set; }
}