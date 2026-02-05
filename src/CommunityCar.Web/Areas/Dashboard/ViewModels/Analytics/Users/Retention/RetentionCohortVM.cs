namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Analytics.Users.Retention;

/// <summary>
/// Retention cohort view model
/// </summary>
public class RetentionCohortVM
{
    public DateTime CohortDate { get; set; }
    public int InitialSize { get; set; }
    public Dictionary<int, double> RetentionRates { get; set; } = new(); // Period -> Rate
}




