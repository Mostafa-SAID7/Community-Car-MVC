namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.Users.General;

/// <summary>
/// User engagement report view model
/// </summary>
public class UserEngagementReportVM
{
    public DateTime ReportDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double OverallEngagementRate { get; set; }
    public int TotalInteractions { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalShares { get; set; }
    public int TotalViews { get; set; }
    public Dictionary<string, double> EngagementByContentType { get; set; } = new();
    public Dictionary<string, int> InteractionsByType { get; set; } = new();
    public List<string> TopEngagingUsers { get; set; } = new();
    public List<string> TopEngagingContent { get; set; } = new();
}




