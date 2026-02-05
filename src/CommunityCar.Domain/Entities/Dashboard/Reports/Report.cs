using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Dashboard;

namespace CommunityCar.Domain.Entities.Dashboard.Reports;

public class Report : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public ReportFormat Format { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int DownloadCount { get; set; }
    public bool IsPublic { get; set; }
    public string Parameters { get; set; } = string.Empty; // JSON serialized parameters
    public string Data { get; set; } = string.Empty; // JSON serialized report data
    
    // Navigation properties
    public List<ReportSchedule> Schedules { get; set; } = new();
}