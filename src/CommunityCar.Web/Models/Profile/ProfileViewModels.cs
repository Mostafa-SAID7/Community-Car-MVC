using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CommunityCar.Web.Models.Profile;

public class ProfileIndexVM
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
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool IsActive { get; set; }
    
    // OAuth connections
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    
    // Statistics
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesReceived { get; set; }
}

public class ProfileSettingsVM
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    [Display(Name = "Bio")]
    public string? Bio { get; set; }

    [StringLength(100)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [Display(Name = "Country")]
    public string? Country { get; set; }

    [StringLength(500)]
    [Display(Name = "Bio (Arabic)")]
    public string? BioAr { get; set; }

    [StringLength(100)]
    [Display(Name = "City (Arabic)")]
    public string? CityAr { get; set; }

    [StringLength(100)]
    [Display(Name = "Country (Arabic)")]
    public string? CountryAr { get; set; }

    [Display(Name = "Profile Picture")]
    public IFormFile? ProfilePicture { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    
    // Notification settings
    [Display(Name = "Email Notifications")]
    public bool EmailNotifications { get; set; }
    
    [Display(Name = "Push Notifications")]
    public bool PushNotifications { get; set; }
    
    [Display(Name = "SMS Notifications")]
    public bool SmsNotifications { get; set; }
    
    [Display(Name = "Marketing Emails")]
    public bool MarketingEmails { get; set; }
    
    // OAuth connections
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    
    // Security
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
}

public class UpdateProfileVM
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Bio")]
    public string? Bio { get; set; }

    [StringLength(100)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [Display(Name = "Country")]
    public string? Country { get; set; }

    [StringLength(500)]
    [Display(Name = "Bio (Arabic)")]
    public string? BioAr { get; set; }

    [StringLength(100)]
    [Display(Name = "City (Arabic)")]
    public string? CityAr { get; set; }

    [StringLength(100)]
    [Display(Name = "Country (Arabic)")]
    public string? CountryAr { get; set; }

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
}

public class ChangePasswordVM
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UpdateEmailVM
{
    [Required]
    [EmailAddress]
    [Display(Name = "New Email")]
    public string NewEmail { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}

public class TwoFactorSetupVM
{
    public string SecretKey { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    public List<string> BackupCodes { get; set; } = new();
    
    [Required]
    [Display(Name = "Verification Code")]
    public string Code { get; set; } = string.Empty;
}

public class SecurityVM
{
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
    public List<SecurityLogItemVM> RecentActivity { get; set; } = new();
}

public class SecurityLogItemVM
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
}

public class PrivacySettingsVM
{
    [Display(Name = "Profile Visible to Public")]
    public bool ProfileVisible { get; set; } = true;
    
    [Display(Name = "Email Visible to Others")]
    public bool EmailVisible { get; set; }
    
    [Display(Name = "Phone Number Visible to Others")]
    public bool PhoneVisible { get; set; }
    
    [Display(Name = "Allow Direct Messages")]
    public bool AllowMessages { get; set; } = true;
    
    [Display(Name = "Allow Friend Requests")]
    public bool AllowFriendRequests { get; set; } = true;
}

public class DeleteAccountVM
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "I understand this action cannot be undone")]
    public bool ConfirmDeletion { get; set; }
}