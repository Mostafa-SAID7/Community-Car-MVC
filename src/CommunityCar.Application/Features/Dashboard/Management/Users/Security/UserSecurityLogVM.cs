namespace CommunityCar.Application.Features.Dashboard.Management.Users.Security;

/// <summary>
/// ViewModel for user security logs in management context
/// </summary>
public class UserSecurityLogVM
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty; // Debug, Info, Warning, Error, Critical
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestId { get; set; }
    public string? CorrelationId { get; set; }
    public string? Action { get; set; }
    public string? Resource { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public string? Location { get; set; }
    public string? Device { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public bool IsSuspicious { get; set; }
    public string? RiskLevel { get; set; }
    public string? SessionId { get; set; }
}