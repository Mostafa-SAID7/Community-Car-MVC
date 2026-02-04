namespace CommunityCar.Application.Features.Dashboard.Audit.ViewModels;

/// <summary>
/// ViewModel for audit log entries
/// </summary>
public class AuditLogVM
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? AdditionalData { get; set; }
    public string Severity { get; set; } = string.Empty; // Info, Warning, Error
    public string Category { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
    public bool IsSuccessful { get; set; } = true;
    public string? ErrorMessage { get; set; }
}