using CommunityCar.Application.Features.Dashboard.Management.ViewModels;
using CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Management;

public interface IManagementService
{
    Task<DashboardOverviewVM> GetDashboardOverviewAsync();
    Task<List<CommunityCar.Application.Features.Dashboard.Management.ViewModels.SystemTaskVM>> GetSystemTasksAsync();
    Task<bool> ExecuteSystemTaskAsync(string taskType, Dictionary<string, object>? parameters = null);
    Task<bool> CancelSystemTaskAsync(Guid taskId);
    Task<CommunityCar.Application.Features.Dashboard.Management.ViewModels.SystemTaskVM?> GetSystemTaskAsync(Guid taskId);
    Task<List<CommunityCar.Application.Features.Dashboard.Management.ViewModels.SystemLogVM>> GetSystemLogsAsync(int page = 1, int pageSize = 50, string? level = null);
    Task<bool> ClearSystemLogsAsync(DateTime? olderThan = null);
    Task<CommunityCar.Application.Features.Dashboard.Management.ViewModels.SystemResourcesVM> GetSystemResourcesAsync();
    Task<List<ChartDataVM>> GetSystemMetricsChartAsync(string metricType, int hours = 24);
    Task<bool> RestartSystemServiceAsync(string serviceName);
    Task<bool> UpdateSystemConfigurationAsync(Dictionary<string, object> configuration);
    Task<Dictionary<string, object>> GetSystemConfigurationAsync();
}