namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Activity;

public class ActivityHeatmapVM
{
    public Dictionary<int, int> HourlyData { get; set; } = new();
    public Dictionary<string, int> DailyData { get; set; } = new();
    public Dictionary<string, int> WeeklyData { get; set; } = new();
    public Dictionary<string, int> MonthlyData { get; set; } = new();
    public int TotalActivities { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}




