using System.ComponentModel.DataAnnotations;

using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Account.ViewModels.Core;

public class UserEngagementStatsVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public double OverallEngagementRate { get; set; }
    public int TotalInteractions { get; set; }
    public int LikesReceived { get; set; }
    public int CommentsReceived { get; set; }
    public int SharesReceived { get; set; }
    public int ViewsReceived { get; set; }
    public int LikesGiven { get; set; }
    public int CommentsGiven { get; set; }
    public int SharesGiven { get; set; }
    public double AveragePostEngagement { get; set; }
    public double AverageCommentEngagement { get; set; }
    public int ActiveDaysThisMonth { get; set; }
    public int ActiveDaysThisWeek { get; set; }
    public DateTime LastEngagementAt { get; set; }
    public List<ChartDataVM> EngagementTrend { get; set; } = new();
    public Dictionary<string, double> EngagementByContentType { get; set; } = new();
    public List<string> TopEngagingContent { get; set; } = new();
}