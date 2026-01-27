using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Infrastructure.Models.TwoFactor;

namespace CommunityCar.Infrastructure.Services.Authentication.TwoFactor;

/// <summary>
/// Interface for authenticator-based 2FA operations
/// </summary>
public interface IAuthenticatorService
{
    Task<Result> EnableTwoFactorAsync(EnableTwoFactorRequest request);
    Task<Result> DisableTwoFactorAsync(DisableTwoFactorRequest request);
    Task<string> GenerateQrCodeAsync(GenerateQrCodeRequest request);
    Task<Result> VerifyTwoFactorTokenAsync(VerifyTwoFactorTokenRequest request);
    Task<bool> IsTwoFactorEnabledAsync(string userId);
}
