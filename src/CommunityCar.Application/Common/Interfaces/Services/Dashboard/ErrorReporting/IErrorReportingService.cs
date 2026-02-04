using CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.ErrorReporting;

public interface IErrorReportingService
{
    Task<List<ErrorReportVM>> GetErrorReportsAsync(int page = 1, int pageSize = 20, string? severity = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<ErrorReportVM?> GetErrorReportByIdAsync(Guid errorId);
    Task<Guid> CreateErrorReportAsync(CreateErrorReportVM request);
    Task<bool> ResolveErrorReportAsync(Guid errorId, string resolution, string resolvedBy);
    Task<bool> DeleteErrorReportAsync(Guid errorId);
    Task<ErrorStatisticsVM> GetErrorStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ChartDataVM>> GetErrorTrendChartAsync(int days = 30);
    Task<List<ErrorReportVM>> GetTopErrorsAsync(int limit = 10, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ErrorReportVM>> GetRecentErrorsAsync(int limit = 20);
    Task<bool> BulkResolveErrorsAsync(List<Guid> errorIds, string resolution, string resolvedBy);
    Task<bool> BulkDeleteErrorsAsync(List<Guid> errorIds);
    Task<List<ErrorReportVM>> SearchErrorsAsync(string query, int page = 1, int pageSize = 20);
    Task<bool> ExportErrorReportsAsync(DateTime startDate, DateTime endDate, string format = "csv");
    Task<bool> ConfigureErrorAlertsAsync(ErrorAlertConfigVM config);
    Task<ErrorAlertConfigVM> GetErrorAlertConfigAsync();
}