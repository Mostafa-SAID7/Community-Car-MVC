using CommunityCar.Application.Features.Dashboard.Management.Users.Security;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Security;

public interface IUserSecurityRepository
{
    Task<List<UserSecurityLogVM>> GetUserSecurityLogsAsync(string userId, int page = 1, int pageSize = 20);
    Task<UserSecurityManagementVM?> GetUserSecurityManagementAsync(string userId);
    Task<bool> LockUserAsync(string userId, string reason, DateTime? lockoutEnd = null);
    Task<bool> UnlockUserAsync(string userId);
    Task<bool> EnableTwoFactorAsync(string userId);
    Task<bool> DisableTwoFactorAsync(string userId);
    Task<List<UserSecurityLogVM>> GetSuspiciousActivitiesAsync(int page = 1, int pageSize = 20);
    Task<bool> LogSecurityEventAsync(string userId, string eventType, string details, string ipAddress);
    Task<int> GetFailedLoginAttemptsAsync(string userId, DateTime since);
    Task<bool> ResetFailedLoginAttemptsAsync(string userId);
}