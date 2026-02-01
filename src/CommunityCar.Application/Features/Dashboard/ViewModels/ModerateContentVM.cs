namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class ModerateContentVM
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "approve", "reject", "flag"
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public bool NotifyUser { get; set; } = true;
}