using System.ComponentModel.DataAnnotations;
using CommunityCar.Application.Common.Models.Identity;

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
}

public class UpdatePrivacySettingsRequest
{
    public Guid UserId { get; set; }
    public bool ProfileVisible { get; set; } = true;
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
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
}

public class TwoFactorSetupRequest
{
    public string SecretKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

#endregion

#region View Models

public class AccountInfoVM : UserSummaryVM
{
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsDeactivated { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public string? DeactivationReason { get; set; }
}

public class PrivacySettingsVM
{
    public Guid UserId { get; set; }
    public bool ProfileVisible { get; set; } = true;
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
    public bool ShowOnlineStatus { get; set; } = true;
    public bool AllowTagging { get; set; } = true;
    public bool ShowActivityStatus { get; set; } = true;
    public bool DataProcessingConsent { get; set; }
    public bool MarketingEmailsConsent { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class NotificationSettingsVM
{
    public Guid UserId { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool PushEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = false;
    public bool MarketingEnabled { get; set; } = false;
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

public class ProfileSettingsVM : UserSummaryVM
{
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? BioAr { get; set; }
    public string? CityAr { get; set; }
    public string? CountryAr { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
}

public class ActiveSessionVM
{
    public string SessionId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = "Unknown";
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public bool IsCurrent { get; set; }
}

public class SecurityLogVM
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Location { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SecurityInfoVM
{
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
    public bool HasOAuthLinked { get; set; }
}

public class TwoFactorSetupVM
{
    public string SecretKey { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
    public List<string> RecoveryCodes { get; set; } = new();
}

#endregion
