using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Models.Authentication;
using CommunityCar.Domain.Entities.Auth;
using CommunityCar.Infrastructure.Models.TwoFactor;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

namespace CommunityCar.Infrastructure.Services.Authentication.TwoFactor;

/// <summary>
/// Service for authenticator-based 2FA operations
/// </summary>
public class AuthenticatorService : IAuthenticatorService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthenticatorService> _logger;

    public AuthenticatorService(
        UserManager<User> userManager,
        ILogger<AuthenticatorService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> EnableTwoFactorAsync(EnableTwoFactorRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // Generate authenticator key if not exists
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            // Enable two-factor authentication
            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure("Failed to enable two-factor authentication.", errors);
            }

            user.IsTwoFactorEnabled = true;
            user.TwoFactorEnabledAt = DateTime.UtcNow;
            user.TwoFactorSecretKey = key;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Two-factor authentication enabled for user {UserId}", request.UserId);
            return Result.Success("Two-factor authentication has been enabled successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling two-factor authentication for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while enabling two-factor authentication.");
        }
    }

    public async Task<Result> DisableTwoFactorAsync(DisableTwoFactorRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // Verify current password
            if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
            {
                return Result.Failure("Invalid password.");
            }

            // Disable two-factor authentication
            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure("Failed to disable two-factor authentication.", errors);
            }

            user.IsTwoFactorEnabled = false;
            user.TwoFactorSecretKey = null;
            user.BackupCodes = null;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Two-factor authentication disabled for user {UserId}", request.UserId);
            return Result.Success("Two-factor authentication has been disabled successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling two-factor authentication for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while disabling two-factor authentication.");
        }
    }

    public async Task<string> GenerateQrCodeAsync(GenerateQrCodeRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var email = await _userManager.GetEmailAsync(user);
            var issuer = "CommunityCar";
            
            var qrCodeUri = $"otpauth://totp/{UrlEncoder.Default.Encode(issuer)}:{UrlEncoder.Default.Encode(email)}?secret={key}&issuer={UrlEncoder.Default.Encode(issuer)}";
            
            _logger.LogInformation("QR code generated for user {UserId}", request.UserId);
            return qrCodeUri;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating QR code for user {UserId}", request.UserId);
            throw;
        }
    }

    public async Task<Result> VerifyTwoFactorTokenAsync(VerifyTwoFactorTokenRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, request.Token);
            
            if (isValid)
            {
                _logger.LogInformation("Two-factor token verified successfully for user {UserId}", request.UserId);
                return Result.Success("Token verified successfully.");
            }

            _logger.LogWarning("Invalid two-factor token for user {UserId}", request.UserId);
            return Result.Failure("Invalid token.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying two-factor token for user {UserId}", request.UserId);
            return Result.Failure("An error occurred while verifying the token.");
        }
    }

    public async Task<bool> IsTwoFactorEnabledAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.IsTwoFactorEnabled ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking two-factor status for user {UserId}", userId);
            return false;
        }
    }
}
