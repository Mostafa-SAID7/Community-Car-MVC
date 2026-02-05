using CommunityCar.Application.Features.Shared.ViewModels.Security;
using CommunityCar.Application.Features.Shared.ViewModels.Users;
using CommunityCar.Application.Features.Dashboard.Reports.Users.Security.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview.Users.Security;

public interface IUserOverviewSecurityService
{
    Task<SecurityOverviewVM> GetSecurityOverviewAsync();
    Task<List<SecurityThreatVM>> GetRecentThreatsAsync(int count = 10);
    Task<List<UserSecurityEventVM>> GetRecentSecurityEventsAsync(int count = 20);
    Task<SecurityOverviewVM> GetSecurityStatsAsync();
}