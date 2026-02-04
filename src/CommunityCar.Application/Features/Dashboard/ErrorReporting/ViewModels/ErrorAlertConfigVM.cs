using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class ErrorAlertConfigVM
{
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    
    [Required]
    public string ErrorLevel { get; set; } = string.Empty; // Error, Warning, Critical
    
    public string? Category { get; set; }
    public int ThresholdCount { get; set; } = 1;
    public int TimeWindowMinutes { get; set; } = 60;
    public List<string> Recipients { get; set; } = new();
    public string NotificationMethod { get; set; } = "Email"; // Email, SMS, Slack
    public Dictionary<string, object> Settings { get; set; } = new();
}