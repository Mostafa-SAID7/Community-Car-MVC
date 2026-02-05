using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;

namespace CommunityCar.Web.Areas.Identity.Interfaces.Services.Authentication;

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


