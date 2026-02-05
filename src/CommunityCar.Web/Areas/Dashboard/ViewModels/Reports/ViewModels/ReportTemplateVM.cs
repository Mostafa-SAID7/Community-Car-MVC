namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.ViewModels;

public class ReportTemplateVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<ReportFieldVM> Fields { get; set; } = new();
    public List<ReportChartVM> Charts { get; set; } = new();
    public Dictionary<string, object> DefaultParameters { get; set; } = new();
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}




