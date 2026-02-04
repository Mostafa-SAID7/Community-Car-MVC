using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Monitoring;
using CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;
using CommunityCar.Application.Common.Interfaces.Services.Community.Moderation;
using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Application.Features.Dashboard.System.ViewModels;

namespace CommunityCar.Application.Services.Dashboard.Monitoring;

public class MonitoringService : IMonitoringService
{
    public async Task<SystemHealthVM> GetSystemHealthAsync()
    {
        // Mock data - return a single SystemHealthVM instead of a list
        var systemHealth = new SystemHealthVM
        {
            CheckTime = DateTime.UtcNow,
            ServiceName = "System",
            Status = "Healthy",
            OverallStatus = "Healthy",
            ResponseTime = (int)TimeSpan.FromMilliseconds(245.5).TotalMilliseconds,
            CpuUsage = 35.2m,
            MemoryUsage = 68.7m,
            DiskUsage = 45.3m,
            ActiveConnections = 127,
            ErrorCount = 0,
            WarningCount = 0,
            Uptime = TimeSpan.FromHours(720.5),
            Version = "1.0.0",
            Environment = "Production",
            IsHealthy = true,
            LastCheck = DateTime.UtcNow,
            Issues = new List<SystemIssueVM>()
        };

        return await Task.FromResult(systemHealth);
    }

    public async Task<SystemHealthVM?> GetServiceHealthAsync(string serviceName)
    {
        var systemHealth = await GetSystemHealthAsync();
        return systemHealth.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase) ? systemHealth : null;
    }

    public async Task<bool> UpdateSystemHealthAsync(string serviceName, string status, double responseTime,
        double cpuUsage, double memoryUsage, double diskUsage, int activeConnections, int errorCount)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<List<ChartDataVM>> GetPerformanceChartAsync(string serviceName, DateTime startDate, DateTime endDate)
    {
        var data = new List<ChartDataVM>();
        var random = new Random();
        var current = startDate;

        while (current <= endDate)
        {
            data.Add(new ChartDataVM
            {
                Label = current.ToString("HH:mm"),
                Value = (double)(random.NextDouble() * 100 + 50),
                Date = current
            });
            current = current.AddHours(1);
        }

        return await Task.FromResult(data);
    }

    public async Task<bool> IsSystemHealthyAsync()
    {
        var systemHealth = await GetSystemHealthAsync();
        return systemHealth.IsHealthy;
    }

    public async Task<List<SystemAlertVM>> GetSystemAlertsAsync(int page, int pageSize)
    {
        var alerts = new List<SystemAlertVM>();
        for (int i = 0; i < pageSize; i++)
        {
            alerts.Add(new SystemAlertVM
            {
                Id = Guid.NewGuid(),
                Title = $"System Alert {i + 1}",
                Message = $"Sample alert {i + 1}",
                Severity = i % 3 == 0 ? "High" : "Medium",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i * 10),
                IsRead = false,
                Type = i % 2 == 0 ? "Security" : "System"
            });
        }
        return await Task.FromResult(alerts);
    }

    public async Task<List<CommunityCar.Application.Common.Interfaces.Services.Community.Moderation.ModerationItemVM>> GetContentModerationAsync(int page, int pageSize)
    {
        var items = new List<CommunityCar.Application.Common.Interfaces.Services.Community.Moderation.ModerationItemVM>();
        for (int i = 0; i < pageSize; i++)
        {
            items.Add(new CommunityCar.Application.Common.Interfaces.Services.Community.Moderation.ModerationItemVM
            {
                Id = Guid.NewGuid(),
                ContentType = "Comment",
                Content = "This is a sample content for moderation.",
                Author = $"User{i}",
                CreatedAt = DateTime.UtcNow.AddHours(-i),
                Status = "Pending",
                ReportCount = i % 5
            });
        }
        return await Task.FromResult(items);
    }

    public async Task<List<SystemAlertVM>> GetUnreadAlertsAsync()
    {
        var alerts = await GetSystemAlertsAsync(1, 5);
        return alerts.Where(a => !a.IsRead).ToList();
    }

    public async Task<bool> MarkAlertAsReadAsync(Guid id)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> MarkAllAlertsAsReadAsync()
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> DeleteAlertAsync(Guid id)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ModerateContentAsync(Guid id, string action, string? reason)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ModeratContentAsync(Guid id, string action, string? reason)
    {
        return await ModerateContentAsync(id, action, reason);
    }
}