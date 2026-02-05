using CommunityCar.Application.Features.Dashboard.Reports.Users.Security;
using CommunityCar.Application.Features.Dashboard.Management.Users.Security;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Security;

public interface IUserManagementSecurityRepository
{
    Task<SecurityStatsVM> GetSecurityStatsAsync();
    Task<List<UserSecurityLogVM>> GetSecurityLogsAsync(int page = 1, int pageSize = 20);
    Task<bool> LogSecurityEventAsync(string userId, string eventType, string description);
}