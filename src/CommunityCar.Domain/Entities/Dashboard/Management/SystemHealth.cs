using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Management;

public class SystemHealth : BaseEntity
{
    public DateTime CheckTime { get; set; }
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public decimal DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public bool IsHealthy { get; set; }
    public string? Issues { get; set; }

    public SystemHealth()
    {
        CheckTime = DateTime.UtcNow;
    }

    public void UpdateHealth(decimal cpuUsage, decimal memoryUsage, decimal diskUsage,
        int activeConnections, TimeSpan responseTime, int errorCount, int warningCount,
        string? issues = null)
    {
        CpuUsage = cpuUsage;
        MemoryUsage = memoryUsage;
        DiskUsage = diskUsage;
        ActiveConnections = activeConnections;
        ResponseTime = responseTime;
        ErrorCount = errorCount;
        WarningCount = warningCount;
        Issues = issues;
        IsHealthy = cpuUsage < 80 && memoryUsage < 80 && diskUsage < 90 && errorCount < 10;
        CheckTime = DateTime.UtcNow;
        Audit(UpdatedBy);
    }
}

public class UserManagementOverview : BaseEntity
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime LastLogin { get; set; }
    public int LoginCount { get; set; }
    public int PostCount { get; set; }
    public int CommentCount { get; set; }
    public int ReportCount { get; set; }
    public string? Notes { get; set; }

    public void UpdateActivity(DateTime lastLogin, int loginCount, int postCount, int commentCount)
    {
        LastLogin = lastLogin;
        LoginCount = loginCount;
        PostCount = postCount;
        CommentCount = commentCount;
        Audit(UpdatedBy);
    }

    public void BlockUser(string reason)
    {
        IsBlocked = true;
        Notes = $"Blocked: {reason} at {DateTime.UtcNow}";
        Audit(UpdatedBy);
    }

    public void UnblockUser()
    {
        IsBlocked = false;
        Notes = $"Unblocked at {DateTime.UtcNow}";
        Audit(UpdatedBy);
    }
}