using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account;

#region Profile Management

public class ProfileVM
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesReceived { get; set; }
}

public class EditProfileVM
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
    [Display(Name = "Bio")]
    public string? Bio { get; set; }

    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    [Display(Name = "Location")]
    public string? Location { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL")]
    [StringLength(200, ErrorMessage = "Website URL cannot exceed 200 characters")]
    [Display(Name = "Website")]
    public string? Website { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public string? CoverImageUrl { get; set; }
}

#endregion

#region Gallery Management

public class GalleryVM
{
    public List<GalleryItemVM> Images { get; set; } = new();
}

public class GalleryItemVM
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsProfilePicture { get; set; }
    public bool IsCoverImage { get; set; }
}

#endregion

#region Gamification

public class BadgesVM
{
    public List<BadgeVM> Badges { get; set; } = new();
    public List<AchievementVM> Achievements { get; set; } = new();
}

public class BadgeVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; }
    public bool IsRare { get; set; }
}

public class AchievementVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int Target { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

#endregion

#region Settings

public class AccountSettingsVM
{
    [Display(Name = "Email Notifications")]
    public bool EmailNotifications { get; set; }

    [Display(Name = "Push Notifications")]
    public bool PushNotifications { get; set; }

    [Display(Name = "Private Profile")]
    public bool PrivateProfile { get; set; }

    [Display(Name = "Show Email")]
    public bool ShowEmail { get; set; }

    [Display(Name = "Show Location")]
    public bool ShowLocation { get; set; }

    [Display(Name = "Language")]
    public string Language { get; set; } = "en-US";

    [Display(Name = "Time Zone")]
    public string TimeZone { get; set; } = "UTC";
}

#endregion

#region Security

public class TwoFactorVM
{
    public bool IsEnabled { get; set; }
    public int RecoveryCodesCount { get; set; }
}

public class DisableTwoFactorVM
{
    [Required(ErrorMessage = "Password is required to disable two-factor authentication")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}

public class LinkAccountVM
{
    [Required]
    public string Token { get; set; } = string.Empty;
}

public class UnlinkAccountVM
{
    [Required]
    public string Provider { get; set; } = string.Empty;
}

#endregion

#region Account Management

public class DeactivateAccountVM
{
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    [Display(Name = "Reason (Optional)")]
    public string? Reason { get; set; }
}

public class DeleteAccountVM
{
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    [Display(Name = "Reason (Optional)")]
    public string? Reason { get; set; }

    [Required(ErrorMessage = "You must confirm account deletion")]
    [Display(Name = "I understand that this action cannot be undone")]
    public bool ConfirmDeletion { get; set; }
}

public class ExportDataVM
{
    [Display(Name = "Include Profile Information")]
    public bool IncludeProfile { get; set; } = true;

    [Display(Name = "Include Posts")]
    public bool IncludePosts { get; set; } = true;

    [Display(Name = "Include Comments")]
    public bool IncludeComments { get; set; } = true;

    [Display(Name = "Include Messages")]
    public bool IncludeMessages { get; set; } = true;

    [Display(Name = "Include Activity Log")]
    public bool IncludeActivity { get; set; } = false;
}

public class PrivacySettingsVM
{
    [Display(Name = "Profile Visibility")]
    public string ProfileVisibility { get; set; } = "Public"; // Public, Friends, Private

    [Display(Name = "Show Email Address")]
    public bool ShowEmail { get; set; }

    [Display(Name = "Show Location")]
    public bool ShowLocation { get; set; }

    [Display(Name = "Show Online Status")]
    public bool ShowOnlineStatus { get; set; }

    [Display(Name = "Allow Messages from Strangers")]
    public bool AllowMessagesFromStrangers { get; set; }

    [Display(Name = "Allow Tagging")]
    public bool AllowTagging { get; set; }

    [Display(Name = "Show Activity Status")]
    public bool ShowActivityStatus { get; set; }

    [Display(Name = "Data Processing Consent")]
    public bool DataProcessingConsent { get; set; }

    [Display(Name = "Marketing Emails Consent")]
    public bool MarketingEmailsConsent { get; set; }
}

#endregion


