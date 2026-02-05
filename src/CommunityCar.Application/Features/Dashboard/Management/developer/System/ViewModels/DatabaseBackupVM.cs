namespace CommunityCar.Application.Features.Dashboard.Management.developer.System.ViewModels;

public class DatabaseBackupVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Full, Incremental, Differential
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
}