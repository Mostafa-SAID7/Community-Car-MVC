using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Common.Models.Account;

#region Request Models

public class DeactivateAccountRequest
{
    public Guid UserId { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    public string? Reason { get; set; }
}

public class DeleteAccountRequest
{
    public Guid UserId { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    
    public string? Reason { get; set; }
}

public class ExportUserDataRequest
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
    public List<string> DataTypes { get; set; } = new();
    public string Format { get; set; } = "JSON";
    public bool IncludeDeleted { get; set; }
    public bool IncludeProfile { get; set; } = true;
    public bool IncludePosts { get; set; } = true;
    public bool IncludeComments { get; set; } = true;
    public bool IncludeMessages { get; set; } = true;
    public bool IncludeActivity { get; set; } = true;
}

public class UpdatePrivacySettingsRequest
{
    public Guid UserId { get; set; }
    public bool ProfileVisible { get; set; } = true;
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
    public bool AllowMessages { get; set; } = true;
    public bool AllowFriendRequests { get; set; } = true;
    public bool ShowOnlineStatus { get; set; } = true;
    public bool AllowTagging { get; set; } = true;
    public bool ShowActivityStatus { get; set; } = true;
}

public class UpdateNotificationSettingsRequest
{
    public Guid UserId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; }
    public bool MarketingEmails { get; set; }
    public bool CommentNotifications { get; set; } = true;
    public bool LikeNotifications { get; set; } = true;
    public bool FollowNotifications { get; set; } = true;
    public bool MessageNotifications { get; set; } = true;
}

#endregion

#region View Models

public class AccountInfoVM
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeactivated { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public string? DeactivationReason { get; set; }
}

public class PrivacySettingsVM
{
    public Guid UserId { get; set; }
    public string ProfileVisibility { get; set; } = "Public";
    public bool ShowEmail { get; set; }
    public bool ShowLocation { get; set; }
    public bool ShowOnlineStatus { get; set; }
    public bool AllowMessagesFromStrangers { get; set; }
    public bool AllowTagging { get; set; }
    public bool ShowActivityStatus { get; set; }
    public bool DataProcessingConsent { get; set; }
    public bool MarketingEmailsConsent { get; set; }
    public DateTime LastUpdated { get; set; }

    // Legacy/Compatibility fields
    public bool ProfileVisible { get; set; }
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
    public bool AllowMessages { get; set; }
    public bool AllowFriendRequests { get; set; }
}

public class DataExportVM
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string? DownloadUrl { get; set; }
    public long FileSize { get; set; }
}

public class ConsentVM
{
    public string Type { get; set; } = string.Empty;
    public bool IsAccepted { get; set; }
    public DateTime AcceptedAt { get; set; }
    public string Version { get; set; } = "1.0";
}

public class ProfileSettingsVM
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? BioAr { get; set; }
    public string? CityAr { get; set; }
    public string? CountryAr { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
}

#endregion


