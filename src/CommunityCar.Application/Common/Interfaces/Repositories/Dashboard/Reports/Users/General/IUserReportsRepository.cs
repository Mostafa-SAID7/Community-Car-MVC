using CommunityCar.Application.Features.Dashboard.Reports.Users.General;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports.Users.General;

public interface IUserReportsRepository
{
    Task<UserReportVM> GenerateUserReportAsync(DateTime startDate, DateTime endDate);
    Task<List<UserActivityReportVM>> GetUserActivityReportAsync(DateTime startDate, DateTime endDate);
    Task<UserEngagementReportVM> GetEngagementReportAsync(DateTime startDate, DateTime endDate);
    Task<List<UserRetentionReportVM>> GetRetentionReportAsync(DateTime startDate, DateTime endDate);
}