namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class DataExportVM
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "Pending";
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public long FileSize { get; set; }
}