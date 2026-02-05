namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports.Users.General;

public interface IUserReportsService
{
    Task<CommunityCar.Application.Features.Shared.ViewModels.Users.UserActivityVM> GenerateUserReportAsync(DateTime startDate, DateTime endDate);
    Task<List<CommunityCar.Application.Features.Shared.ViewModels.Users.UserActivityVM>> GetUserActivityReportAsync(DateTime startDate, DateTime endDate);
    Task<CommunityCar.Application.Features.Account.ViewModels.Core.UserEngagementStatsVM> GetEngagementReportAsync(DateTime startDate, DateTime endDate);
    Task<List<CommunityCar.Application.Features.Shared.ViewModels.Users.UserActivityVM>> GetRetentionReportAsync(DateTime startDate, DateTime endDate);
}