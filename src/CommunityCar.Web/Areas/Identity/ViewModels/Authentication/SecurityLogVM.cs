namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class SecurityLogVM
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string StatusIcon { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}
