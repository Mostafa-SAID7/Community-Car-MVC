using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models;

using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;
using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

using CommunityCar.Application.Common.Models.Account;
using CommunityCar.Application.Common.Models.Profile;

namespace CommunityCar.Application.Common.Interfaces.Services.Account;

/// <summary>
/// Interface for account security operations (password, 2FA, security logs)
/// </summary>
public interface IAccountSecurityService
{
    // Security Information
    Task<SecurityInfoVM> GetSecurityInfoAsync(Guid userId);
    Task<IEnumerable<SecurityLogVM>> GetSecurityLogAsync(Guid userId, int page = 1, int pageSize = 20);

    // Password Management
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<bool> ValidatePasswordAsync(Guid userId, string password);
    Task<DateTime?> GetLastPasswordChangeAsync(Guid userId);

    // Session Management
    Task<IEnumerable<ActiveSessionVM>> GetActiveSessionsAsync(Guid userId);
    Task<bool> RevokeSessionAsync(Guid userId, string sessionId);
    Task<bool> RevokeAllSessionsAsync(Guid userId);

    // Security Logs
    Task LogSecurityEventAsync(Guid userId, string action, string? ipAddress = null, string? userAgent = null, bool isSuccessful = true);
    Task<bool> ClearSecurityLogAsync(Guid userId);

    // Account Lockout
    Task<bool> IsAccountLockedAsync(Guid userId);
    Task<DateTime?> GetLockoutEndAsync(Guid userId);
    Task<bool> UnlockAccountAsync(Guid userId);

    // Two-Factor Authentication
    Task<TwoFactorSetupVM> SetupTwoFactorAsync(Guid userId);
    Task<bool> EnableTwoFactorAsync(Guid userId, TwoFactorSetupRequest request);
    Task<bool> DisableTwoFactorAsync(Guid userId, string password);
    Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code);
}


