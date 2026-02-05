namespace CommunityCar.Application.Features.Shared.ViewModels.Security;

/// <summary>
/// Base security event view model - shared between Account and Dashboard
/// </summary>
public class SecurityEventVM
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public string Location { get; set; } = string.Empty;
    public Dictionary<string, object> EventData { get; set; } = new();
}