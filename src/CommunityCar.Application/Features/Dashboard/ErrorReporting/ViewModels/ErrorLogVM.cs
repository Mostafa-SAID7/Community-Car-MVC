using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class ErrorLogVM
{
    public Guid Id { get; set; }
    public string ErrorId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty; // Error, Warning, Info, Debug
    public string Category { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public int OccurrenceCount { get; set; }
    public DateTime FirstOccurrence { get; set; }
    public DateTime LastOccurrence { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}