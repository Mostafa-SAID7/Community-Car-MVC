namespace CommunityCar.Application.Features.Dashboard.Management.ViewModels;

/// <summary>
/// ViewModel for system logs in management context
/// </summary>
public class SystemLogVM
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty; // Debug, Info, Warning, Error, Critical
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestId { get; set; }
    public string? CorrelationId { get; set; }
}