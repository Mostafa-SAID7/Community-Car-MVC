using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Monitoring;
using CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;
using CommunityCar.Application.Common.Interfaces.Services.Community.Moderation;
using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Application.Features.Dashboard.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Services.Dashboard.Monitoring;

public class MonitoringService : IMonitoringService
{
    public async Task<List<SystemHealthVM>> GetSystemHealthAsync()
    {
        // Mock data
        var services = new List<SystemHealthVM>
        {
            new()
            {
                CheckTime = DateTime.UtcNow,
                ServiceName = "Web Application",
                Status = "Healthy",
                ResponseTime = 245.5,
                CpuUsage = 35.2,
                MemoryUsage = 68.7,
                DiskUsage = 45.3,
                ActiveConnections = 127,
                ErrorCount = 0,
                Uptime = 720.5,
                Version = "1.0.0",
                Environment = "Production",
                IsHealthy = true
            },
            new()
            {
                CheckTime = DateTime.UtcNow,
                ServiceName = "Database",
                Status = "Healthy",
                ResponseTime = 12.3,
                CpuUsage = 22.1,
                MemoryUsage = 78.9,
                DiskUsage = 62.4,
                ActiveConnections = 45,
                ErrorCount = 0,
                Uptime = 1440.2,
                Version = "15.2",
                Environment = "Production",
                IsHealthy = true
            }
        };

        return await Task.FromResult(services);
    }

    public async Task<SystemHealthVM?> GetServiceHealthAsync(string serviceName)
    {
        var allServices = await GetSystemHealthAsync();
        return allServices.FirstOrDefault(s => s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
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
                Value = (decimal)(random.NextDouble() * 100 + 50),
                Date = current
            });
            current = current.AddHours(1);
        }

        return await Task.FromResult(data);
    }

    public async Task<bool> IsSystemHealthyAsync()
    {
        var services = await GetSystemHealthAsync();
        return services.All(s => s.IsHealthy);
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