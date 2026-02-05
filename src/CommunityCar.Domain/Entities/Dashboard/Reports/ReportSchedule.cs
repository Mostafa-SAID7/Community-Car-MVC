using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Dashboard;

namespace CommunityCar.Domain.Entities.Dashboard.Reports;

public class ReportSchedule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType ReportType { get; set; }
    public ScheduleFrequency Frequency { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public DateTime NextRun { get; set; }
    public DateTime? LastRun { get; set; }
    public bool IsActive { get; set; }
    public string Recipients { get; set; } = string.Empty; // JSON serialized list of email addresses
    public ReportFormat Format { get; set; }
    public string Template { get; set; } = string.Empty; // JSON serialized ReportGenerationVM
    
    // Foreign key
    public Guid? ReportId { get; set; }
    
    // Navigation properties
    public Report? Report { get; set; }
}