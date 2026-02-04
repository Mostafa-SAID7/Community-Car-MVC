using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class LogErrorVM
{
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    
    [Required]
    public string Level { get; set; } = string.Empty;
    
    public string Category { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}