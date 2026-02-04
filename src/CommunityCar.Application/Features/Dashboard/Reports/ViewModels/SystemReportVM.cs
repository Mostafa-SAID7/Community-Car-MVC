namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class SystemReportVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime GeneratedDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string GeneratedByName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public int DownloadCount { get; set; }
    public bool IsPublic { get; set; }
    public ReportDataVM Data { get; set; } = new();
    public List<ReportSectionVM> Sections { get; set; } = new();
}

public class ReportGenerationVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> IncludedSections { get; set; } = new();
    public string Format { get; set; } = string.Empty;
    public bool IncludeCharts { get; set; }
    public bool IncludeRawData { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class ReportScheduleVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty; // daily, weekly, monthly
    public string CronExpression { get; set; } = string.Empty;
    public DateTime NextRun { get; set; }
    public DateTime? LastRun { get; set; }
    public bool IsActive { get; set; }
    public List<string> Recipients { get; set; } = new();
    public string Format { get; set; } = string.Empty;
    public ReportGenerationVM Template { get; set; } = new();
}

public class ReportDataVM
{
    public Dictionary<string, object> Summary { get; set; } = new();
    public List<Dictionary<string, object>> Details { get; set; } = new();
    public List<ChartDataVM> Charts { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ReportSectionVM
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object Data { get; set; } = new();
    public List<ChartDataVM> Charts { get; set; } = new();
    public int Order { get; set; }
}

public class ChartDataVM
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTime? Date { get; set; }
    public string Color { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}