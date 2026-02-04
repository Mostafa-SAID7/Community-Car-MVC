using CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Monitoring;

/// <summary>
/// Interface for monitoring services
/// </summary>
public interface IMonitoringService
{
    Task<List<SystemHealthVM>> GetSystemHealthAsync(CancellationToken cancellationToken = default);
    Task<SystemHealthVM?> GetServiceHealthAsync(string serviceName, CancellationToken cancellationToken = default);
    Task<List<SystemAlertVM>> GetSystemAlertsAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<List<CommunityCar.Application.Common.Interfaces.Services.Community.Moderation.ModerationItemVM>> GetContentModerationAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<List<SystemAlertVM>> GetUnreadAlertsAsync(CancellationToken cancellationToken = default);
    Task<bool> MarkAlertAsReadAsync(Guid alertId, CancellationToken cancellationToken = default);
    Task<bool> ResolveAlertAsync(Guid alertId, string resolution, CancellationToken cancellationToken = default);
    Task<MonitoringStatsVM> GetMonitoringStatsAsync(CancellationToken cancellationToken = default);
    Task<bool> CreateSystemAlertAsync(string title, string message, string severity, CancellationToken cancellationToken = default);
    Task<bool> UpdateServiceStatusAsync(string serviceName, string status, CancellationToken cancellationToken = default);
}