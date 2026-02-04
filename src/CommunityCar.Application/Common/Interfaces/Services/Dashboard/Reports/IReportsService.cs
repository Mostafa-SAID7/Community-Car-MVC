using CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Reports;

public interface IReportsService
{
    Task<List<SystemReportVM>> GetReportsAsync(int page = 1, int pageSize = 20);
    Task<SystemReportVM?> GetReportByIdAsync(Guid reportId);
    Task<Guid> GenerateReportAsync(ReportGenerationVM request);
    Task<bool> DeleteReportAsync(Guid reportId);
    Task<byte[]> DownloadReportAsync(Guid reportId);
    Task<List<SystemReportVM>> GetReportsByTypeAsync(string reportType, int page = 1, int pageSize = 20);
    Task<bool> ScheduleReportAsync(ReportScheduleVM request);
    Task<List<SystemReportVM>> GetScheduledReportsAsync();
}