namespace CommunityCar.Application.Features.Dashboard.Monitoring.ViewModels;

public class ModerationItemVM
{
    public Guid Id { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string ReportReason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ReportedAt { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? Action { get; set; }
}