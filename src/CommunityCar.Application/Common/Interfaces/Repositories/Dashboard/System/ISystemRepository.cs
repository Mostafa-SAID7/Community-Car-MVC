using CommunityCar.Application.Features.Dashboard.Management.developer.System.ViewModels;
using CommunityCar.Application.Features.Dashboard.Management.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.System;

public interface ISystemRepository
{
    Task<SystemHealthVM> GetSystemHealthAsync();
    Task<List<SystemLogVM>> GetSystemLogsAsync(int page = 1, int pageSize = 20);
    Task<SystemStatsVM> GetSystemStatsAsync();
    Task<List<SystemAlertVM>> GetSystemAlertsAsync(int count = 10);
    Task<bool> CreateSystemAlertAsync(SystemAlertVM alert);
    Task<bool> LogSystemEventAsync(string eventType, string message, string? details = null);
}