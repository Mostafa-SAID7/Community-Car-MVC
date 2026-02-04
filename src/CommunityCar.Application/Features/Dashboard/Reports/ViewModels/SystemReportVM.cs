namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class SystemReportVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string Format { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int DownloadCount { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}