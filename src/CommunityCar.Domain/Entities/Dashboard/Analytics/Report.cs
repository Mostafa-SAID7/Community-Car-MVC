using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Analytics;

public enum ReportType
{
    UserActivity = 0,
    ContentAnalytics = 1,
    SystemPerformance = 2,
    SecurityAudit = 3,
    CustomReport = 4
}

public enum ReportStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}

public class Report : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Parameters { get; set; } = string.Empty; // JSON
    public string? FilePath { get; set; }
    public string? FileUrl { get; set; }
    public long? FileSize { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid RequestedBy { get; set; }

    public Report()
    {
        Status = ReportStatus.Pending;
    }

    public void StartProcessing()
    {
        Status = ReportStatus.Processing;
        Audit(UpdatedBy);
    }

    public void Complete(string filePath, string fileUrl, long fileSize)
    {
        Status = ReportStatus.Completed;
        FilePath = filePath;
        FileUrl = fileUrl;
        FileSize = fileSize;
        CompletedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void Fail(string errorMessage)
    {
        Status = ReportStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void Cancel()
    {
        Status = ReportStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }
}

public class ReportSchedule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ReportType ReportType { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public string Parameters { get; set; } = string.Empty; // JSON
    public bool IsActive { get; set; }
    public DateTime? LastRun { get; set; }
    public DateTime? NextRun { get; set; }
    public Guid ScheduleCreatedBy { get; set; } // Renamed to avoid hiding BaseEntity.CreatedBy
    public List<string> Recipients { get; set; } = new();

    public void UpdateNextRun(DateTime nextRun)
    {
        NextRun = nextRun;
        Audit(UpdatedBy);
    }

    public void RecordRun()
    {
        LastRun = DateTime.UtcNow;
        Audit(UpdatedBy);
    }
}