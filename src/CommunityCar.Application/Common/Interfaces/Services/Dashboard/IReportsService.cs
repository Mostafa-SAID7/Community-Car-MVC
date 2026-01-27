using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IReportsService
{
    Task<List<SystemReportVM>> GetReportsAsync(int page = 1, int pageSize = 20);
    Task<SystemReportVM?> GetReportByIdAsync(Guid reportId);
    Task<Guid> GenerateReportAsync(ReportGenerationRequest request);
    Task<bool> DeleteReportAsync(Guid reportId);
    Task<byte[]> DownloadReportAsync(Guid reportId);
    Task<List<SystemReportVM>> GetReportsByTypeAsync(string reportType, int page = 1, int pageSize = 20);
    Task<bool> ScheduleReportAsync(ReportScheduleRequest request);
    Task<List<SystemReportVM>> GetScheduledReportsAsync();
}


