namespace CommunityCar.Application.Features.Dashboard.Error.ViewModels;

public class ErrorBoundaryVM
{
    public Guid ErrorId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
    public bool IsResolved { get; set; }
    public string Severity { get; set; } = "Error";
}