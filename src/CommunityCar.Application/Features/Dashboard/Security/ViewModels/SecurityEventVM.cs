namespace CommunityCar.Application.Features.Dashboard.Security.ViewModels;

public class SecurityEventVM
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Details { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}