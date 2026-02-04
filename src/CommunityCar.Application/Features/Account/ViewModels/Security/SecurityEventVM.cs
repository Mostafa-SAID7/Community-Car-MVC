namespace CommunityCar.Application.Features.Account.ViewModels.Security;

/// <summary>
/// ViewModel for security events
/// </summary>
public class SecurityEventVM
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty; // Login, PasswordChange, TwoFactorEnabled, etc.
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}