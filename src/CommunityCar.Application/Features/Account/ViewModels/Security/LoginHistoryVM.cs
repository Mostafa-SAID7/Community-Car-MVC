namespace CommunityCar.Application.Features.Account.ViewModels.Security;

/// <summary>
/// ViewModel for login history
/// </summary>
public class LoginHistoryVM
{
    public Guid Id { get; set; }
    public DateTime LoginDate { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public bool IsSuspicious { get; set; }
    public bool IsCurrentSession { get; set; }
    public DateTime? LogoutDate { get; set; }
}