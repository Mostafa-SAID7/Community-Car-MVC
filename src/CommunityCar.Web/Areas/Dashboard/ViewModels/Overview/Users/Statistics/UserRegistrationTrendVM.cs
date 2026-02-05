namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Statistics;

/// <summary>
/// User registration trend view model
/// </summary>
public class UserRegistrationTrendVM
{
    public DateTime Date { get; set; }
    public int NewRegistrations { get; set; }
    public int CumulativeRegistrations { get; set; }
    public double GrowthRate { get; set; }
    public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly
}




