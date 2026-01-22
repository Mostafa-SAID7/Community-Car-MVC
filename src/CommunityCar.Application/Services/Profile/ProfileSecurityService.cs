using CommunityCar.Application.Common.Interfaces.Services.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Profile.DTOs;
using CommunityCar.Application.Features.Profile.ViewModels;
using Microsoft.AspNetCore.Identity;
using CommunityCar.Domain.Entities.Auth;
using System.Security.Cryptography;
using System.Text;

namespace CommunityCar.Application.Services.Profile;

public class ProfileSecurityService : IProfileSecurityService
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public ProfileSecurityService(
        UserManager<User> userManager,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (result.Succeeded)
        {
            user.Audit(_currentUserService.UserId);
            await _userManager.UpdateAsync(user);
        }

        return result.Succeeded;
    }

    public async Task<bool> ValidateCurrentPasswordAsync(Guid userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> UpdateEmailAsync(Guid userId, UpdateEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        // Validate current password
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return false;

        // Generate email change token
        var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
        var result = await _userManager.ChangeEmailAsync(user, request.NewEmail, token);

        if (result.Succeeded)
        {
            user.UserName = request.NewEmail; // Update username to match email
            user.Audit(_currentUserService.UserId);
            await _userManager.UpdateAsync(user);
        }

        return result.Succeeded;
    }

    public async Task<bool> SendEmailConfirmationAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        // In real app, send email with confirmation link
        // await _emailService.SendEmailConfirmationAsync(user.Email, token);
        
        return true;
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<TwoFactorSetupVM> SetupTwoFactorAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new InvalidOperationException("User not found");

        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var email = user.Email ?? string.Empty;
        var qrCodeUrl = GenerateQrCodeUrl(email, key);

        return new TwoFactorSetupVM
        {
            SecretKey = key ?? string.Empty,
            QrCodeUrl = qrCodeUrl,
            ManualEntryKey = FormatKey(key ?? string.Empty)
        };
    }

    public async Task<bool> EnableTwoFactorAsync(Guid userId, TwoFactorSetupRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var isValidToken = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, request.Code);

        if (isValidToken)
        {
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            user.EnableTwoFactor(await _userManager.GetAuthenticatorKeyAsync(user) ?? string.Empty);
            
            // Generate backup codes
            var backupCodes = await GenerateBackupCodesAsync(userId);
            user.SetBackupCodes(System.Text.Json.JsonSerializer.Serialize(backupCodes));
            
            user.Audit(_currentUserService.UserId);
            await _userManager.UpdateAsync(user);
            
            return true;
        }

        return false;
    }

    public async Task<bool> DisableTwoFactorAsync(Guid userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        if (!await _userManager.CheckPasswordAsync(user, password))
            return false;

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);
        
        user.DisableTwoFactor();
        user.Audit(_currentUserService.UserId);
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<List<string>> GenerateBackupCodesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new List<string>();

        var codes = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            codes.Add(GenerateBackupCode());
        }

        return codes;
    }

    public async Task<bool> LinkGoogleAccountAsync(Guid userId, string googleId, string? profilePictureUrl = null)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.LinkGoogleAccount(googleId, profilePictureUrl);
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> LinkFacebookAccountAsync(Guid userId, string facebookId, string? profilePictureUrl = null)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.LinkFacebookAccount(facebookId, profilePictureUrl);
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UnlinkGoogleAccountAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.UnlinkGoogleAccount();
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UnlinkFacebookAccountAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.UnlinkFacebookAccount();
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<List<SecurityLogVM>> GetSecurityLogAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        // Placeholder implementation - in real app, query security logs
        return await Task.FromResult(new List<SecurityLogVM>());
    }

    private static string GenerateQrCodeUrl(string email, string key)
    {
        var issuer = "CommunityCar";
        var encodedIssuer = Uri.EscapeDataString(issuer);
        var encodedEmail = Uri.EscapeDataString(email);
        return $"otpauth://totp/{encodedIssuer}:{encodedEmail}?secret={key}&issuer={encodedIssuer}";
    }

    private static string FormatKey(string key)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < key.Length)
        {
            result.Append(key.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < key.Length)
        {
            result.Append(key.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private static string GenerateBackupCode()
    {
        var bytes = new byte[4];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return BitConverter.ToUInt32(bytes, 0).ToString("D8");
    }
}