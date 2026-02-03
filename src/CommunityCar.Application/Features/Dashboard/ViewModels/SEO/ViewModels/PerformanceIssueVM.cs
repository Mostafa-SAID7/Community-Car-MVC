namespace CommunityCar.Application.Features.SEO.ViewModels;

public class PerformanceIssueVM
{
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public decimal Impact { get; set; }
    public string Fix { get; set; } = string.Empty;
}