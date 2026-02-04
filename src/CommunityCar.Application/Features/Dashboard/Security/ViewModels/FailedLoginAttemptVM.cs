namespace CommunityCar.Application.Features.Dashboard.Security.ViewModels;

public class FailedLoginAttemptVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime AttemptTime { get; set; }
    public string FailureReason { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsSuspicious { get; set; }
    public bool IsBlocked { get; set; }
    public int AttemptCount { get; set; }
}