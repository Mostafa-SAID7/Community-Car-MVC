namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class StatsVM
{
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
    public int ActiveUsersThisWeek { get; set; }
    public int ActiveUsersThisMonth { get; set; }
    public decimal GrowthRate { get; set; }
    public decimal EngagementRate { get; set; }
    public DateTime LastUpdated { get; set; }

    // Properties for Quick Stats widgets
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal ChangePercentage { get; set; }
    public bool IsPositiveChange { get; set; }
}