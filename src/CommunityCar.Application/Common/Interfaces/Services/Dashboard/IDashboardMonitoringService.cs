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
}