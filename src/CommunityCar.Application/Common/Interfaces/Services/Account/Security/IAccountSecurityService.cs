using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Account.ViewModels.Authentication;

namespace CommunityCar.Application.Common.Interfaces.Services.Account.Security;

public interface IAccountSecurityService
{
    // Password Management
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request);
    Task<bool> ValidatePasswordAsync(Guid userId, string password);
    Task<DateTime?> GetLastPasswordChangeAsync(Guid userId);

    // Session Management
    Task<IEnumerable<ActiveSessionVM>> GetActiveSessionsAsync(Guid userId);
    Task<bool> RevokeSessionAsync(Guid userId, string sessionId);
    Task<bool> RevokeAllSessionsAsync(Guid userId);

    // Account Lockout
    Task<bool> IsAccountLockedAsync(Guid userId);
    Task<DateTime?> GetLockoutEndAsync(Guid userId);
    Task<bool> UnlockAccountAsync(Guid userId);

    // Security Logs
    Task<IEnumerable<SecurityLogVM>> GetSecurityLogAsync(Guid userId, int page = 1, int pageSize = 20);
    Task LogSecurityEventAsync(Guid userId, string action, string? ipAddress = null, string? userAgent = null, bool isSuccessful = true);
    Task<bool> ClearSecurityLogAsync(Guid userId);

    // Security Information
    Task<SecurityInfoVM> GetSecurityInfoAsync(Guid userId);
    Task<Result> UpdateLastLoginAsync(Guid userId, string? ipAddress = null, string? userAgent = null);

    // Two-Factor Authentication
    Task<TwoFactorSetupVM> SetupTwoFactorAsync(Guid userId);
    Task<bool> EnableTwoFactorAsync(Guid userId, TwoFactorSetupRequest request);
    Task<bool> DisableTwoFactorAsync(Guid userId, string password);
    Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code);
}