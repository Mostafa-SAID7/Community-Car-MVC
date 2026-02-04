namespace CommunityCar.Application.Features.Dashboard.System.ViewModels;

public class LogFileVM
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string Type { get; set; } = string.Empty;
    public int LineCount { get; set; }
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
}