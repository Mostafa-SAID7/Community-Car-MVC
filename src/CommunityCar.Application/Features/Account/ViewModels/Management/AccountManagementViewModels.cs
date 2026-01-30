using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class DeactivateAccountVM
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
}

public class DeleteAccountVM
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Display(Name = "I confirm that I want to permanently delete my account and all associated data.")]
    public bool ConfirmDeletion { get; set; }
}

public class ExportDataVM
{
    [Display(Name = "Include Profile Data")]
    public bool IncludeProfile { get; set; } = true;

    [Display(Name = "Include Activity Logs")]
    public bool IncludeActivity { get; set; } = true;

    [Display(Name = "Include Media")]
    public bool IncludeMedia { get; set; } = false;
}

public class DeactivateAccountRequest
{
    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class DeleteAccountRequest
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class ExportUserDataRequest
{
    public Guid UserId { get; set; }
    public List<string> DataCategories { get; set; } = new();
    public string Format { get; set; } = "JSON"; 
}

public class DataExportVM
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "Pending";
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public long FileSize { get; set; }
}

public class PrivacySettingsVM
{
    public bool IsPublic { get; set; } = true;
    public bool AllowMessages { get; set; }
    public bool AllowFriendRequests { get; set; }
    public string DefaultGalleryPrivacy { get; set; } = "public";
    public bool ShowActivityStatus { get; set; }
    public bool ShowOnlineStatus { get; set; }
    public bool ProfileVisible { get; set; } = true;
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
}

public class UpdatePrivacySettingsRequest : PrivacySettingsVM
{
    public Guid UserId { get; set; }
}

public class NotificationSettingsVM
{
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public bool WeeklyDigest { get; set; }
    public bool MarketingEmails { get; set; }
    public bool SecurityAlerts { get; set; } = true;
}

public class UpdateNotificationSettingsRequest : NotificationSettingsVM
{
    public Guid UserId { get; set; }
}

public class AccountInfoVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string AccountStatus { get; set; } = "Active";
}

public class ConsentVM
{
    public string Type { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsAccepted { get; set; }
    public DateTime AcceptedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}

public class UserManagementActionVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // Added
    public string Reason { get; set; } = string.Empty;
    public Guid PerformedBy { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime CreatedAt => ActionDate; // Added
    public string? ManagerName { get; set; }
    public string? TargetUserName { get; set; }
    public string? ManagerProfilePicture { get; set; }
    public string? TargetUserProfilePicture { get; set; }
    public string? TimeAgo { get; set; }
    public string? ActionIcon { get; set; }
    public string? ActionColor { get; set; }
    public string? Notes { get; set; }
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

public class CreateManagementActionRequest
{
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Guid PerformedBy { get; set; }
    public string? Notes { get; set; }
    public bool IsReversible { get; set; } = true;
}

public class AssignManagerRequest
{
    public Guid UserId { get; set; }
    public Guid ManagerId { get; set; }
    public string? Role { get; set; }
    public string? Reason { get; set; }
}
