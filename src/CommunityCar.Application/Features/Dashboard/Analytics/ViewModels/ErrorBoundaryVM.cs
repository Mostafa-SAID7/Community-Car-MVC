using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class ErrorBoundaryVM
{
    public Guid Id { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public int OccurrenceCount { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}