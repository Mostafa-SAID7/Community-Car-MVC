using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class ReportScheduleVM
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    public string Format { get; set; } = string.Empty;
    
    [Required]
    public string Schedule { get; set; } = string.Empty; // Cron expression
    
    public List<string> Recipients { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime? NextRun { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}