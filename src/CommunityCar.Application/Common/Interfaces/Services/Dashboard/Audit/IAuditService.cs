using CommunityCar.Application.Features.Dashboard.ViewModels;

using CommunityCar.Application.Features.Dashboard.Audit.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Audit;

public interface IAuditService
{
    Task<List<AuditLogVM>> GetAuditLogsAsync(DateTime? startDate = null, DateTime? endDate = null, string? userId = null, string? action = null, int page = 1, int pageSize = 50);
    Task<AuditLogVM?> GetAuditLogByIdAsync(Guid id);
    Task LogActionAsync(string userId, string action, string entityType, string entityId, string? oldValues = null, string? newValues = null, string? ipAddress = null);
    Task<List<string>> GetAuditActionsAsync();
    Task<List<string>> GetAuditEntityTypesAsync();
    Task<AuditStatisticsVM> GetAuditStatisticsAsync(DateTime startDate, DateTime endDate);
    Task<List<ChartDataVM>> GetAuditActivityChartAsync(int days);
    Task<List<UserAuditSummaryVM>> GetTopActiveUsersAsync(DateTime startDate, DateTime endDate, int limit = 10);
    Task<bool> ExportAuditLogsAsync(DateTime startDate, DateTime endDate, string format = "csv");
    Task<bool> PurgeOldAuditLogsAsync(int daysToKeep);
    Task<List<AuditLogVM>> GetUserAuditHistoryAsync(string userId, int limit = 100);
    Task<List<AuditLogVM>> GetEntityAuditHistoryAsync(string entityType, string entityId, int limit = 50);
}