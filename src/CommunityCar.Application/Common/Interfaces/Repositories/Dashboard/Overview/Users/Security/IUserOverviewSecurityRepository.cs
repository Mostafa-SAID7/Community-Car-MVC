using CommunityCar.Application.Features.Dashboard.Reports.Users.Security.ViewModels;
using CommunityCar.Application.Features.Dashboard.Reports.Users.Security;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Overview.Users.Security;

public interface IUserOverviewSecurityRepository
{
    Task<CommunityCar.Application.Features.Dashboard.Overview.Users.Security.SecurityOverviewVM> GetSecurityOverviewAsync();
    Task<List<SecurityThreatVM>> GetRecentThreatsAsync(int count = 10);
    Task<List<SecurityEventVM>> GetRecentSecurityEventsAsync(int count = 20);
    Task<SecurityStatsVM> GetSecurityStatsAsync();
}