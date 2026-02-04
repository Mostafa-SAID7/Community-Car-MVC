using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.Reports.ViewModels;

public class ReportGenerationVM
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    public string Format { get; set; } = string.Empty;
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string> IncludeFields { get; set; } = new();
    public List<ReportFilterVM> Filters { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool IncludeCharts { get; set; } = true;
    public bool IncludeSummary { get; set; } = true;
}