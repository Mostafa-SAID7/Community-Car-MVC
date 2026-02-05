namespace CommunityCar.Application.Features.Dashboard.Reports.Media.SoftDelete.ViewModels;

public class DeletionHistoryVM
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty; // Deleted, Restored, PermanentlyDeleted
    public string PerformedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}
