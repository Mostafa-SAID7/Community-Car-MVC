using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Common.Models.Authentication;

public class EnableTwoFactorRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;
}

public class DisableTwoFactorRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Current password is required for security")]
    public string CurrentPassword { get; set; } = string.Empty;
}

public class VerifyTwoFactorTokenRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Verification code is required")]
    [StringLength(10, MinimumLength = 6, ErrorMessage = "Verification code must be between 6 and 10 characters")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Verification code must contain only numbers")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Provider is required")]
    [RegularExpression(@"^(Authenticator|SMS|Email)$", ErrorMessage = "Provider must be 'Authenticator', 'SMS', or 'Email'")]
    public string Provider { get; set; } = string.Empty;
}

public class SendSmsTokenRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Phone number must be in international format")]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class VerifyRecoveryCodeRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Recovery code is required")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Recovery code must be between 8 and 20 characters")]
    public string RecoveryCode { get; set; } = string.Empty;
}

public class GenerateRecoveryCodesRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Current password is required for security")]
    public string CurrentPassword { get; set; } = string.Empty;
}


