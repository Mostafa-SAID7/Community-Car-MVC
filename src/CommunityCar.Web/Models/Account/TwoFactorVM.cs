using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account;

public class TwoFactorVM
{
    public bool IsEnabled { get; set; }
    public bool IsMachineRemembered { get; set; }
    public int RecoveryCodesLeft { get; set; }
    public List<string> RecoveryCodes { get; set; } = new();
    public string AuthenticatorKey { get; set; } = string.Empty;
    public string AuthenticatorUri { get; set; } = string.Empty;
}

public class EnableTwoFactorVM
{
    public string QrCodeUri { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Verification code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be 6 digits")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must be 6 digits")]
    [Display(Name = "Verification Code")]
    public string VerificationCode { get; set; } = string.Empty;
}

public class VerifyTwoFactorVM
{
    [Required(ErrorMessage = "Verification code is required")]
    [StringLength(10, MinimumLength = 6, ErrorMessage = "Verification code must be between 6 and 10 digits")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Verification code must contain only numbers")]
    [Display(Name = "Verification Code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Provider { get; set; } = string.Empty;

    public bool RememberMachine { get; set; }
}

public class RecoveryCodesVM
{
    public string[] RecoveryCodes { get; set; } = Array.Empty<string>();
}

public class VerifyRecoveryCodeVM
{
    [Required(ErrorMessage = "Recovery code is required")]
    [Display(Name = "Recovery Code")]
    public string RecoveryCode { get; set; } = string.Empty;
}

public class SendSmsTokenVM
{
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;
}
