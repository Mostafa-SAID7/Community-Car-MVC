using CommunityCar.Application.Features.Dashboard.Reports.Users.Security.ViewModels;
using CommunityCar.Application.Features.Dashboard.Reports.Users.Security;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports.Users.Audit;

public interface IUserAuditReportsService
{
    Task<List<AuditLogVM>> GetAuditReportAsync(DateTime startDate, DateTime endDate, string? userId = null);
    Task<UserSecurityReportVM> GenerateUserAuditReportAsync(string userId, DateTime startDate, DateTime endDate);
    Task<SecurityStatsVM> GetAuditStatsAsync(DateTime startDate, DateTime endDate);
    Task<List<AuditLogVM>> GetSuspiciousActivitiesReportAsync(DateTime startDate, DateTime endDate);
}