namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.ViewModels;

public class ReportFilterVM
{
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty; // Equals, Contains, GreaterThan, etc.
    public object Value { get; set; } = new();
}




