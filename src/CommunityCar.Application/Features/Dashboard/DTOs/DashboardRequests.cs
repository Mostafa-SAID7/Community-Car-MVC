using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.DTOs;

public class ModerateContentRequest
{
    [Required]
    public Guid ContentId { get; set; }
    
    [Required]
    public string ContentType { get; set; } = string.Empty;
    
    [Required]
    public string Action { get; set; } = string.Empty; // approve, reject, delete
    
    public string? Reason { get; set; }
    
    public string? Notes { get; set; }
}

public class CreateReportRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string ReportType { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class UpdateSystemSettingRequest
{
    [Required]
    public string Key { get; set; } = string.Empty;
    
    [Required]
    public string Value { get; set; } = string.Empty;
}

public class UserSearchRequest
{
    public string? Search { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsBlocked { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class AnalyticsRequest
{
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    public string? MetricType { get; set; }
    public string? GroupBy { get; set; } // day, week, month
    public string? ContentType { get; set; } // Added for content analytics
}

public class SystemHealthRequest
{
    public bool IncludeMetrics { get; set; } = true;
    public bool IncludeLogs { get; set; } = false;
    public int LogCount { get; set; } = 100;
}

public class DashboardOverviewRequest
{
    public string? TimeRange { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UserManagementRequest
{
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class ReportGenerationRequest
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class DashboardSettingsRequest
{
    public string Key { get; set; } = string.Empty;
    public string SettingKey { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class ReportScheduleRequest
{
    [Required]
    public string ReportType { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string Schedule { get; set; } = string.Empty; // cron expression
    
    [Required]
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public Dictionary<string, object> Parameters { get; set; } = new();
    
    public bool IsActive { get; set; } = true;
}