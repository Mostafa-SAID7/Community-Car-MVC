namespace CommunityCar.Web.Models.Account.Management;

public class DataExportVM
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string? DownloadUrl { get; set; }
    public long FileSize { get; set; }
    public string Format { get; set; } = "JSON";
    public List<string> DataTypes { get; set; } = new();
}