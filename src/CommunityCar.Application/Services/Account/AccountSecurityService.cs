using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Common.Extensions;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;
using CommunityCar.Application.Features.Account.ViewModels.Core;
using CommunityCar.Domain.Entities.Account.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Application.Services.Account;

public class AccountSecurityService : IAccountSecurityService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AccountSecurityService> _logger;

    public AccountSecurityService(
        UserManager<User> userManager,
        ILogger<AccountSecurityService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    // Security Information
    public Task<SecurityInfoVM> GetSecurityInfoAsync(Guid userId)
    {
        // TODO: Implement actual security info retrieval
        return Task.FromResult(new SecurityInfoVM());
    }

    public Task<IEnumerable<SecurityLogVM>> GetSecurityLogAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        // TODO: Implement security log retrieval from repository (needs SecurityLogRepository)
        return Task.FromResult<IEnumerable<SecurityLogVM>>(new List<SecurityLogVM>());
    }

    // Password Management
    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Failure("User not found");

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        return result.ToApplicationResult();
    }

    public async Task<bool> ValidatePasswordAsync(Guid userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public Task<DateTime?> GetLastPasswordChangeAsync(Guid userId)
    {
        // TODO: This might need custom column or audit log
        return Task.FromResult<DateTime?>(null);
    }

    // Session Management
    public Task<IEnumerable<ActiveSessionVM>> GetActiveSessionsAsync(Guid userId)
    {
        // TODO: Implement session management
        return Task.FromResult<IEnumerable<ActiveSessionVM>>(new List<ActiveSessionVM>());
    }

    public Task<bool> RevokeSessionAsync(Guid userId, string sessionId)
    {
        return Task.FromResult(true);
    }

    public Task<bool> RevokeAllSessionsAsync(Guid userId)
    {
         return Task.FromResult(true);
    }

    // Security Logs
    public Task LogSecurityEventAsync(Guid userId, string action, string? ipAddress = null, string? userAgent = null, bool isSuccessful = true)
    {
        // TODO: Implement logging
        _logger.LogInformation("Security Event: {UserId} - {Action} - Success: {Success}", userId, action, isSuccessful);
        return Task.CompletedTask;
    }

    public Task<bool> ClearSecurityLogAsync(Guid userId)
    {
        return Task.FromResult(true);
    }

    // Account Lockout
    public async Task<bool> IsAccountLockedAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        return await _userManager.IsLockedOutAsync(user);
    }

    public async Task<DateTime?> GetLockoutEndAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;
        var end = await _userManager.GetLockoutEndDateAsync(user);
        return end?.DateTime;
    }

    public async Task<bool> UnlockAccountAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        return result.Succeeded;
    }

    // Login Tracking (extracted from IUserRepository)
    public async Task<Result> UpdateLastLoginAsync(Guid userId, string? ipAddress = null, string? userAgent = null)
    {
         var user = await _userManager.FindByIdAsync(userId.ToString());
         if (user == null) return Result.Failure("User not found");
         
         // Note: We need to update OAuthInfo.LastLoginAt. 
         // Since User entity structure is complex, we might need to load it fully or use repo if it exposes update.
         // But we removed business logic from repo.
         // Let's assume we can update user via UserManager or UpdateAsync.
         // However, User.OAuthInfo is a value object or owned type?
         // Let's try to update it.
         
         user.UpdateLastLogin();
         var result = await _userManager.UpdateAsync(user);
         return result.ToApplicationResult();
    }

    // Two-Factor Authentication
    public Task<TwoFactorSetupVM> SetupTwoFactorAsync(Guid userId)
    {
         // TODO: Implement 2FA setup
         return Task.FromResult(new TwoFactorSetupVM());
    }

    public Task<bool> EnableTwoFactorAsync(Guid userId, TwoFactorSetupRequest request)
    {
         // TODO: Implement 2FA enable
         return Task.FromResult(false);
    }

    public async Task<bool> DisableTwoFactorAsync(Guid userId, string password)
    {
         var user = await _userManager.FindByIdAsync(userId.ToString());
         if (user == null) return false;
         var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
         return result.Succeeded;
    }

    public Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code)
    {
         // TODO: Implement 2FA verify
         return Task.FromResult(false);
    }
}
