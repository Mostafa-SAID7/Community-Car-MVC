using CommunityCar.Application.Common.Interfaces.Services.Authentication;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Infrastructure.Services.Account.TwoFactor;
using CommunityCar.Infrastructure.Models.TwoFactor;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Services.Account.Authentication;

/// <summary>
/// Orchestrator service for two-factor authentication
/// </summary>
public class TwoFactorService : ITwoFactorService
{
    private readonly IAuthenticatorService _authenticatorService;
    private readonly IRecoveryCodesService _recoveryCodesService;
    private readonly ILogger<TwoFactorService> _logger;

    public TwoFactorService(
        IAuthenticatorService authenticatorService,
        IRecoveryCodesService recoveryCodesService,
        ILogger<TwoFactorService> logger)
    {
        _authenticatorService = authenticatorService;
        _recoveryCodesService = recoveryCodesService;
        _logger = logger;
    }

    #region Authenticator - Delegate to AuthenticatorService

    public async Task<Result> EnableTwoFactorAsync(EnableTwoFactorRequest request)
        => await _authenticatorService.EnableTwoFactorAsync(request);

    public async Task<Result> DisableTwoFactorAsync(DisableTwoFactorRequest request)
        => await _authenticatorService.DisableTwoFactorAsync(request);

    public async Task<Result> GenerateQrCodeAsync(string userId)
    {
        var request = new GenerateQrCodeRequest { UserId = userId };
        var qrCodeUri = await _authenticatorService.GenerateQrCodeAsync(request);
        return Result.Success("QR code generated successfully.", new { QrCodeUri = qrCodeUri });
    }

    public async Task<Result> VerifyTwoFactorTokenAsync(VerifyTwoFactorTokenRequest request)
        => await _authenticatorService.VerifyTwoFactorTokenAsync(request);

    public async Task<bool> IsTwoFactorEnabledAsync(string userId)
        => await _authenticatorService.IsTwoFactorEnabledAsync(userId);

    #endregion

    #region Recovery Codes - Delegate to RecoveryCodesService

    public async Task<Result> GenerateRecoveryCodesAsync(GenerateRecoveryCodesRequest request)
        => await _recoveryCodesService.GenerateRecoveryCodesAsync(request);

    public async Task<Result> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request)
        => await _recoveryCodesService.VerifyRecoveryCodeAsync(request);

    #endregion

    #region SMS Token (TODO: Implement SMS service)

    public async Task<Result> SendSmsTokenAsync(SendSmsTokenRequest request)
    {
        // TODO: Implement SMS token service
        _logger.LogInformation("SMS token requested for user {UserId}", request.UserId);
        return Result.Success("SMS token sent successfully.");
    }

    public async Task<Result> VerifySmsTokenAsync(VerifyTwoFactorTokenRequest request)
    {
        // TODO: Implement SMS token verification
        _logger.LogInformation("SMS token verification requested for user {UserId}", request.UserId);
        return Result.Success("SMS token verified successfully.");
    }

    #endregion

    #region Email Token (TODO: Implement Email service)

    public async Task<Result> SendEmailTokenAsync(string userId)
    {
        // TODO: Implement Email token service
        _logger.LogInformation("Email token requested for user {UserId}", userId);
        return Result.Success("Email token sent successfully.");
    }

    public async Task<Result> VerifyEmailTokenAsync(VerifyTwoFactorTokenRequest request)
    {
        // TODO: Implement Email token verification
        _logger.LogInformation("Email token verification requested for user {UserId}", request.UserId);
        return Result.Success("Email token verified successfully.");
    }

    #endregion
}