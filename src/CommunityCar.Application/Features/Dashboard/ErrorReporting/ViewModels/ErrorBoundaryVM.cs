namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class ErrorBoundaryVM
{
    public Guid Id { get; set; }
    public string BoundaryName { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime? OccurredAt { get; set; }
    public bool IsRecovered { get; set; }
    public DateTime? RecoveredAt { get; set; }
    public int FailureCount { get; set; }
    public string? RecoveryAction { get; set; }
    public string? ComponentName { get; set; }
    public string? Severity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastFailureAt { get; set; }
    public string? LastError { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}