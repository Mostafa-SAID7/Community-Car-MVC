using CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;
using CommunityCar.Application.Features.Dashboard.System.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Monitoring;

public interface IMonitoringService
{
    Task<SystemHealthVM> GetSystemHealthAsync();
    Task<SystemHealthVM?> GetServiceHealthAsync(string serviceName);
    Task<bool> UpdateSystemHealthAsync(string serviceName, string status, double responseTime, double cpuUsage, double memoryUsage, double diskUsage, int activeConnections, int errorCount);
    Task<List<ChartDataVM>> GetPerformanceChartAsync(string serviceName, DateTime startDate, DateTime endDate);
    Task<bool> IsSystemHealthyAsync();
    Task<List<SystemAlertVM>> GetSystemAlertsAsync(int page, int pageSize);
    Task<List<CommunityCar.Application.Common.Interfaces.Services.Community.Moderation.ModerationItemVM>> GetContentModerationAsync(int page, int pageSize);
    Task<List<SystemAlertVM>> GetUnreadAlertsAsync();
    Task<bool> MarkAlertAsReadAsync(Guid id);
    Task<bool> MarkAllAlertsAsReadAsync();
    Task<bool> DeleteAlertAsync(Guid id);
    Task<bool> ModerateContentAsync(Guid id, string action, string? reason);
    Task<bool> ModeratContentAsync(Guid id, string action, string? reason);
}