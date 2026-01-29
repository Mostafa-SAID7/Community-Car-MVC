using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Services.Authentication;

public interface ITwoFactorService
{
    Task<Result> EnableTwoFactorAsync(EnableTwoFactorRequest request);
    Task<Result> DisableTwoFactorAsync(DisableTwoFactorRequest request);
    Task<Result> GenerateQrCodeAsync(string userId);
    Task<Result> VerifyTwoFactorTokenAsync(VerifyTwoFactorTokenRequest request);
    Task<Result> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request);
    Task<Result> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request);
    Task<bool> IsTwoFactorEnabledAsync(string userId);
    Task<Result> SendSmsTokenAsync(SendSmsTokenRequest request);
    Task<Result> VerifySmsTokenAsync(VerifyTwoFactorTokenRequest request);
    Task<Result> SendEmailTokenAsync(string userId);
    Task<Result> VerifyEmailTokenAsync(VerifyTwoFactorTokenRequest request);
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


