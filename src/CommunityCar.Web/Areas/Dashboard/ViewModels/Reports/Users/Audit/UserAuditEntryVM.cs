namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Audit;

/// <summary>
/// User audit entry view model
/// </summary>
public class UserAuditEntryVM
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
}




