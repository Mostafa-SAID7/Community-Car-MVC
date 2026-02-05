namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;

public class SecurityCheckVM
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pass, Fail, Warning
    public string Description { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? Recommendation { get; set; }
}




