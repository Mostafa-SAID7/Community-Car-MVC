namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.ViewModels;

public class DashboardStatsVM
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Change { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty; // increase, decrease, neutral
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    
    // Additional properties for dashboard statistics
    public int TotalUsers { get; set; }
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalQuestions { get; set; }
    public int TotalAnswers { get; set; }
    public int TotalReviews { get; set; }
    public int TotalStories { get; set; }
    public int TotalNews { get; set; }
    public int TotalInteractions { get; set; }
    public int ActiveUsersToday { get; set; }
    public double EngagementRate { get; set; }
    public double GrowthRate { get; set; }
    public double ChangePercentage { get; set; }
    public bool IsPositiveChange { get; set; }
}




