namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class UserManagementActionVM
{
    public Guid Id { get; set; }
    public Guid ManagerId { get; set; }
    public Guid TargetUserId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public string TargetUserName { get; set; } = string.Empty;
    public string? ManagerProfilePicture { get; set; }
    public string? TargetUserProfilePicture { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string ActionIcon { get; set; } = string.Empty;
    public string ActionColor { get; set; } = string.Empty;
}

public class ManagementAuditVM
{
    public Guid UserId { get; set; }
    public List<UserManagementActionVM> Actions { get; set; } = new();
    public int TotalActions { get; set; }
    public Dictionary<string, int> ActionsByType { get; set; } = new();
    public DateTime? LastActionDate { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasMore { get; set; }
}