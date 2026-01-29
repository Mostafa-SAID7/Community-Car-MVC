using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using CommunityCar.Web.Models.Account.Settings;

namespace CommunityCar.Web.Models.Account.Core;

/// <summary>
/// Profile information view model focused on user profile data
/// </summary>
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
    
    // Basic confirmation status
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    
    // Notification settings (flattened for easier binding)
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    public bool MarketingEmails { get; set; } = false;
    
    // Security settings
    public bool IsTwoFactorEnabled { get; set; }
    public int ActiveSessions { get; set; }
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    
    // Nested settings objects
    public NotificationSettingsVM NotificationSettings { get; set; } = new();
    public PrivacySettingsVM PrivacySettings { get; set; } = new();
}