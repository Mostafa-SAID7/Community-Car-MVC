namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class ErrorDetailsViewModel
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string? InnerException { get; set; }
    public DateTime Timestamp { get; set; }
    public string? UserId { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public string? RequestPath { get; set; }
    public string? HttpMethod { get; set; }
    public int StatusCode { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}