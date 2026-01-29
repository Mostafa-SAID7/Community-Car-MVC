namespace CommunityCar.Web.Models.Account.Security;

/// <summary>
/// View model for security log items
/// </summary>
public class SecurityLogItemVM
{
    public string Action { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsSuccessful { get; set; }
}