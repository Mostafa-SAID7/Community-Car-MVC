using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class TwoFactorVM
{
    public bool IsEnabled { get; set; }
    public bool IsMachineRemembered { get; set; }
    public int RecoveryCodesLeft { get; set; }

    [Display(Name = "Authenticator Key")]
    public string? AuthenticatorKey { get; set; }

    [Display(Name = "Authenticator URI")]
    public string? AuthenticatorUri { get; set; }
}

public class TwoFactorSetupVM
{
    public string SecretKey { get; set; } = string.Empty;
    public string QrCodeUrl { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    public List<string> BackupCodes { get; set; } = new();
    public bool IsEnabled { get; set; }
    
    // UI specific
    public string? QrCodeUri { get; set; }
}

public class EnableTwoFactorVM
{
    [Required]
    [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Text)]
    [Display(Name = "Verification Code")]
    public string VerificationCode { get; set; } = string.Empty;

    public string? ManualEntryKey { get; set; }
}

public class DisableTwoFactorVM
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class TwoFactorSetupRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}

public class EnableTwoFactorRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Provider { get; set; } = "Authenticator";
}

public class DisableTwoFactorRequest
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
}

public class VerifyTwoFactorTokenRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Provider { get; set; } = "Authenticator";
    public bool RememberMachine { get; set; }
}

public class GenerateRecoveryCodesRequest
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
}

public class VerifyRecoveryCodeRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class SendSmsTokenRequest
{
    public Guid UserId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}

public class TwoFactorChallengeResult
{
    public bool RequiresTwoFactor { get; set; }
    public string[] AvailableProviders { get; set; } = Array.Empty<string>();
    public string ChallengeToken { get; set; } = string.Empty;
}

// Aliases
public delegate void SuccessAction(); // placeholder if needed