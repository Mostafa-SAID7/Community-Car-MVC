namespace CommunityCar.Application.Features.Dashboard.Error.ViewModels;

/// <summary>
/// ViewModel for error boundary information
/// </summary>
public class ErrorBoundaryVM
{
    public Guid Id { get; set; }
    public string ErrorId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Critical, High, Medium, Low
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
    public int OccurrenceCount { get; set; } = 1;
}