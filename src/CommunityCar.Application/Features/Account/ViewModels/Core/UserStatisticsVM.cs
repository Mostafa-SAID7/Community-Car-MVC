namespace CommunityCar.Application.Features.Account.ViewModels.Core;

public class UserStatisticsVM
{
    public Guid UserId { get; set; }
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesReceived { get; set; }
    public int SharesReceived { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int AchievementsCount { get; set; }
    public int BadgesCount { get; set; }
    public int GalleryItemsCount { get; set; }
    public DateTime JoinedDate { get; set; }
    public int DaysActive { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public string JoinedTimeAgo { get; set; } = string.Empty;
    public string LastActivityTimeAgo { get; set; } = string.Empty;
    public int ExperiencePoints { get; set; }
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public int Rank { get; set; }
    
    // Aliases
    public int TotalBadges => BadgesCount;
}