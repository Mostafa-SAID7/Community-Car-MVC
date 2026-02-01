namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class SystemReportVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime GeneratedDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string GeneratedByName { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int DownloadCount { get; set; }
    public long? FileSize { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
}