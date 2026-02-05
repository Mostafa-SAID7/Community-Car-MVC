using CommunityCar.Application.Features.Dashboard.Management.Users.Security;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management.Users.Security;

public interface IUserManagementSecurityService
{
    Task<List<UserSecurityLogVM>> GetUserSecurityLogsAsync(string userId, int page = 1, int pageSize = 20);
    Task<UserSecurityManagementVM> GetUserSecurityManagementAsync(string userId);
    Task<Result> LockUserAsync(string userId, string reason);
    Task<Result> UnlockUserAsync(string userId);
    Task<Result> ResetPasswordAsync(string userId);
    Task<Result> EnableTwoFactorAsync(string userId);
    Task<Result> DisableTwoFactorAsync(string userId);
    Task<List<UserSecurityLogVM>> GetSuspiciousActivitiesAsync(int page = 1, int pageSize = 20);
}