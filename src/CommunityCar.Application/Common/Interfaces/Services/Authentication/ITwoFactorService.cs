using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Services.Authentication;

public interface ITwoFactorService
{
    Task<AuthResult> EnableTwoFactorAsync(EnableTwoFactorRequest request);
    Task<AuthResult> DisableTwoFactorAsync(DisableTwoFactorRequest request);
    Task<AuthResult> GenerateQrCodeAsync(string userId);
    Task<AuthResult> VerifyTwoFactorTokenAsync(VerifyTwoFactorTokenRequest request);
    Task<AuthResult> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    Task<AuthResult> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request);
    Task<bool> IsTwoFactorEnabledAsync(string userId);
    Task<AuthResult> SendSmsTokenAsync(SendSmsTokenRequest request);
    Task<AuthResult> VerifySmsTokenAsync(VerifyTwoFactorTokenRequest request);
    Task<AuthResult> SendEmailTokenAsync(string userId);
    Task<AuthResult> VerifyEmailTokenAsync(VerifyTwoFactorTokenRequest request);
}

public class TwoFactorSetupInfo
{
    public string QrCodeUri { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
    public string[] RecoveryCodes { get; set; } = Array.Empty<string>();
}

public class TwoFactorChallengeResult
{
    public bool RequiresTwoFactor { get; set; }
    public string[] AvailableProviders { get; set; } = Array.Empty<string>();
    public string ChallengeToken { get; set; } = string.Empty;
}