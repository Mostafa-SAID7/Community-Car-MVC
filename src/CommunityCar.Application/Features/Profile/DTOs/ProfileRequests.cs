using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Profile.DTOs;

public class UpdateProfileRequest
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Bio { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(500)]
    public string? BioAr { get; set; }

    [StringLength(100)]
    public string? CityAr { get; set; }

    [StringLength(100)]
    public string? CountryAr { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}

public class ChangePasswordRequest
{
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UpdateEmailRequest
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class TwoFactorSetupRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
}

public class DeleteAccountRequest
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public bool ConfirmDeletion { get; set; }
}

public class UpdateNotificationSettingsRequest
{
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public bool MarketingEmails { get; set; }
}


