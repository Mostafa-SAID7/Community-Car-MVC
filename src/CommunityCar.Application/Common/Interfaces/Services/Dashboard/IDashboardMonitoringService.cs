using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IDashboardMonitoringService
{
    Task<List<SystemHealthVM>> GetSystemHealthAsync();
    Task<SystemHealthVM?> GetServiceHealthAsync(string serviceName);
    Task<bool> UpdateSystemHealthAsync(string serviceName, string status, double responseTime, 
        double cpuUsage, double memoryUsage, double diskUsage, int activeConnections, int errorCount);
    Task<List<ChartDataVM>> GetPerformanceChartAsync(string serviceName, DateTime startDate, DateTime endDate);
    Task<bool> IsSystemHealthyAsync();

    // Added to match MonitoringController expectations
    Task<List<SystemAlertVM>> GetSystemAlertsAsync(int page, int pageSize);
    Task<List<ModerationItemVM>> GetContentModerationAsync(int page, int pageSize);
    Task<List<SystemAlertVM>> GetUnreadAlertsAsync();
    Task<bool> MarkAlertAsReadAsync(Guid id);
    Task<bool> MarkAllAlertsAsReadAsync();
    Task<bool> DeleteAlertAsync(Guid id);
    Task<bool> ModerateContentAsync(Guid id, string action, string? reason);
    Task<bool> ModeratContentAsync(Guid id, string action, string? reason); // Matching the typo in controller for now to avoid multiple file edits if risky, but I'll fix the controller too if possible.
}