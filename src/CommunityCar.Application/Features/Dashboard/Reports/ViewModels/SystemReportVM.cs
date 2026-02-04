namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class SystemReportVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // User, Content, Performance, Security, etc.
    public string Format { get; set; } = string.Empty; // PDF, Excel, CSV, JSON
    public string Status { get; set; } = string.Empty; // Generating, Completed, Failed
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string? DownloadUrl { get; set; }
    public long? FileSize { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}