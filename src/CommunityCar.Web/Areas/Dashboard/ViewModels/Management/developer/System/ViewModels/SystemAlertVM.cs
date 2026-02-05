namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.System.ViewModels;

/// <summary>
/// ViewModel for system alerts
/// </summary>
public class SystemAlertVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Info, Warning, Error, Critical
    public string Category { get; set; } = string.Empty; // System, Security, Performance, etc.
    public DateTime CreatedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolvedBy { get; set; }
    public string? ResolutionNotes { get; set; }
    public string Source { get; set; } = string.Empty; // Service or component that generated the alert
    public Dictionary<string, object> Metadata { get; set; } = new();
    public int Priority { get; set; } = 1; // 1-5, 5 being highest
    public bool RequiresAction { get; set; }
    public string? ActionUrl { get; set; }
    
    // Additional properties used by MonitoringService
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public string Type { get; set; } = string.Empty;
}






