namespace CommunityCar.Application.Features.Dashboard.Error.ViewModels;

public class ErrorBoundaryVM
{
    public Guid Id { get; set; }
    public string BoundaryName { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public bool IsRecovered { get; set; }
    public DateTime? RecoveredAt { get; set; }
    public string? RecoveryAction { get; set; }
    public int OccurrenceCount { get; set; }
    public Dictionary<string, object> ErrorInfo { get; set; } = new();
}