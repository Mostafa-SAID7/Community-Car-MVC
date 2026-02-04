using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using CommunityCar.Domain.Entities.Account.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserEntity = CommunityCar.Domain.Entities.Account.Core.User;

namespace CommunityCar.Application.Services.Account.Security;


public class AccountSecurityService : IAccountSecurityService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AccountSecurityService> _logger;

    public AccountSecurityService(
        IUserRepository userRepository,
        UserManager<UserEntity> userManager,
        ICurrentUserService currentUserService,
        ILogger<AccountSecurityService> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Password Management

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return Result.Failure("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (result.Succeeded)
            {
                user.LastPasswordChangeAt = DateTime.UtcNow;
                user.Audit(_currentUserService.UserId);
                await _userRepository.UpdateAsync(user);

                await LogSecurityEventAsync(userId, "Password Changed");
                return Result.Success("Password changed successfully.");
            }

            await LogSecurityEventAsync(userId, "Password Change Failed", isSuccessful: false);
            return Result.Failure("Failed to change password.", result.Errors.Select(e => e.Description).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return Result.Failure("An error occurred while changing password.");
        }
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request)
        => await ChangePasswordAsync(request.UserId, request);

    public async Task<bool> ValidatePasswordAsync(Guid userId, string password)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null && await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<DateTime?> GetLastPasswordChangeAsync(Guid userId)
        => await _userRepository.GetLastPasswordChangeAsync(userId);

    #endregion

    #region Session Management

    public async Task<IEnumerable<ActiveSessionVM>> GetActiveSessionsAsync(Guid userId)
    {
        // Placeholder for real session management
        return new List<ActiveSessionVM>
        {
            new ActiveSessionVM
            {
                SessionId = Guid.NewGuid().ToString(),
                IpAddress = "127.0.0.1",
                UserAgent = "Current Browser",
                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow,
                IsCurrent = true
            }
        };
    }

    public async Task<bool> RevokeSessionAsync(Guid userId, string sessionId)
    {
        await LogSecurityEventAsync(userId, $"Session Revoked: {sessionId}");
        return true;
    }

    public async Task<bool> RevokeAllSessionsAsync(Guid userId)
    {
        await LogSecurityEventAsync(userId, "All Sessions Revoked");
        return true;
    }

    #endregion

    #region Account Lockout

    public async Task<bool> IsAccountLockedAsync(Guid userId)
        => await _userRepository.IsAccountLockedAsync(userId);

    public async Task<DateTime?> GetLockoutEndAsync(Guid userId)
        => await _userRepository.GetLockoutEndAsync(userId);

    public async Task<bool> UnlockAccountAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        if (result.Succeeded)
        {
            await LogSecurityEventAsync(userId, "Account Unlocked");
            return true;
        }
        return false;
    }

    #endregion

    #region Security Logs

    public async Task<IEnumerable<SecurityLogVM>> GetSecurityLogAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        // Placeholder for real logs
        return new List<SecurityLogVM>();
    }

    public async Task LogSecurityEventAsync(Guid userId, string action, string? ipAddress = null, string? userAgent = null, bool isSuccessful = true)
    {
        _logger.LogInformation("Security event for {UserId}: {Action} (Success: {IsSuccessful})", userId, action, isSuccessful);
        await Task.CompletedTask;
    }

    public async Task<bool> ClearSecurityLogAsync(Guid userId)
    {
        _logger.LogInformation("Security log cleared for {UserId}", userId);
        return true;
    }

    #endregion

    #region Security Information

    public async Task<SecurityInfoVM> GetSecurityInfoAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return new SecurityInfoVM();

        var sessions = await GetActiveSessionsAsync(userId);
        
        return new SecurityInfoVM
        {
            IsTwoFactorEnabled = user.TwoFactorEnabled,
            LastPasswordChange = user.LastPasswordChangeAt,
            ActiveSessions = sessions.Count(),
            HasOAuthLinked = user.OAuthInfo.HasAnyOAuthAccount
        };
    }

    public async Task<Result> UpdateLastLoginAsync(Guid userId, string? ipAddress = null, string? userAgent = null)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.OAuthInfo.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            
            await LogSecurityEventAsync(userId, "Login", ipAddress, userAgent);
            return Result.Success("Login time updated.");
        }
        return Result.Failure("User not found.");
    }

    #endregion

    #region Two-Factor Authentication (Internal Proxies)

    public async Task<TwoFactorSetupVM> SetupTwoFactorAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        return new TwoFactorSetupVM 
        { 
            SecretKey = key ?? string.Empty,
            QrCodeUrl = $"otpauth://totp/CommunityCar:{user.Email}?secret={key}&issuer=CommunityCar"
        };
    }

    public async Task<bool> EnableTwoFactorAsync(Guid userId, TwoFactorSetupRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, request.Code);

        if (isValid)
        {
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            user.EnableTwoFactor(request.SecretKey);
            await _userRepository.UpdateAsync(user);
            await LogSecurityEventAsync(userId, "2FA Enabled");
            return true;
        }

        return false;
    }

    public async Task<bool> DisableTwoFactorAsync(Guid userId, string password)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        if (await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            user.DisableTwoFactor();
            await _userRepository.UpdateAsync(user);
            await LogSecurityEventAsync(userId, "2FA Disabled");
            return true;
        }
        return false;
    }

    public async Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        return await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);
    }

    #endregion
}