namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.Security.ViewModels;

/// <summary>
/// Failed login trend view model
/// </summary>
public class FailedLoginTrendVM
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string Label { get; set; } = string.Empty;
}




