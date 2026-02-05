using CommunityCar.Application.Features.Dashboard.Reports.Users.Security;
using CommunityCar.Application.Features.Dashboard.Reports.Users.Security.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports.Users.Security;

public interface IUserSecurityReportsService
{
    Task<UserSecurityReportVM> GenerateSecurityReportAsync(DateTime startDate, DateTime endDate);
    Task<List<SecurityThreatVM>> GetThreatReportAsync(DateTime startDate, DateTime endDate);
    Task<List<VulnerabilityVM>> GetVulnerabilityReportAsync();
    Task<SecurityStatsVM> GetSecurityStatsReportAsync(DateTime startDate, DateTime endDate);
}