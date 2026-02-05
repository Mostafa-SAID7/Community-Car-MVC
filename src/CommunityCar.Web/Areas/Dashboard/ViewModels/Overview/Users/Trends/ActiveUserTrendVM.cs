namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Trends;

/// <summary>
/// Active user trend view model
/// </summary>
public class ActiveUserTrendVM
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string Label { get; set; } = string.Empty;
}




