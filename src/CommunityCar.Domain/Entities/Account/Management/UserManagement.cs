using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Management;

public class UserManagementOverview : BaseEntity
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime LastLogin { get; set; }
    public int LoginCount { get; set; }
    public int PostCount { get; set; }
    public int CommentCount { get; set; }
    public int ReportCount { get; set; }
    public string? Notes { get; set; }

    public void UpdateActivity(DateTime lastLogin, int loginCount, int postCount, int commentCount)
    {
        LastLogin = lastLogin;
        LoginCount = loginCount;
        PostCount = postCount;
        CommentCount = commentCount;
        Audit(UpdatedBy);
    }

    public void BlockUser(string reason)
    {
        IsBlocked = true;
        Notes = $"Blocked: {reason} at {DateTime.UtcNow}";
        Audit(UpdatedBy);
    }

    public void UnblockUser()
    {
        IsBlocked = false;
        Notes = $"Unblocked at {DateTime.UtcNow}";
        Audit(UpdatedBy);
    }
}