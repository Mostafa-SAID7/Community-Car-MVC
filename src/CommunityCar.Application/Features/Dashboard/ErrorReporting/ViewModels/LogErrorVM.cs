namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class LogErrorVM
{
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string? UserId { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public string? Url { get; set; }
    public string Level { get; set; } = "Error";
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}