using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.ErrorReporting.ViewModels;

public class CreateErrorReportVM
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string Category { get; set; } = string.Empty;
    
    public string Priority { get; set; } = "Medium";
    
    [EmailAddress]
    public string? ReporterEmail { get; set; }
    
    public string? ReporterName { get; set; }
    public List<string> Attachments { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}