using CommunityCar.Application.Features.Dashboard.Maintenance.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.Maintenance;

public interface IMaintenanceService
{
    Task<MaintenanceStatusVM> GetMaintenanceStatusAsync();
    Task<bool> EnableMaintenanceModeAsync(string message, DateTime? scheduledEnd = null);
    Task<bool> DisableMaintenanceModeAsync();
    Task<bool> ScheduleMaintenanceAsync(DateTime startTime, DateTime endTime, string message, bool notifyUsers = true);
    Task<bool> CancelScheduledMaintenanceAsync();
    Task<List<MaintenanceHistoryVM>> GetMaintenanceHistoryAsync(int page = 1, int pageSize = 20);
    Task<bool> UpdateMaintenanceMessageAsync(string message);
    Task<bool> AddAllowedIpAsync(string ipAddress);
    Task<bool> RemoveAllowedIpAsync(string ipAddress);
    Task<List<string>> GetAllowedIpsAsync();
    Task<bool> NotifyUsersAsync(string message, DateTime? scheduledTime = null);
    Task<MaintenanceStatsVM> GetMaintenanceStatsAsync();
    Task<bool> TestMaintenancePageAsync();
    Task<List<ChartDataVM>> GetMaintenanceChartAsync(int months = 12);
}