namespace CommunityCar.Infrastructure.Configuration.Account.Gamification;

/// <summary>
/// Gamification configuration settings
/// </summary>
public class GamificationSettings
{
    public const string SectionName = "Gamification";

    public bool EnableGamification { get; set; } = true;
    public PointsSettings Points { get; set; } = new();
    public BadgeSettings Badges { get; set; } = new();
    public AchievementSettings Achievements { get; set; } = new();
    public LeaderboardSettings Leaderboards { get; set; } = new();
    public RewardSettings Rewards { get; set; } = new();
}

/// <summary>
/// Points system settings
/// </summary>
public class PointsSettings
{
    public bool EnablePoints { get; set; } = true;
    public int PostCreationPoints { get; set; } = 10;
    public int CommentPoints { get; set; } = 5;
    public int LikeReceivedPoints { get; set; } = 2;
    public int ShareReceivedPoints { get; set; } = 3;
    public int ProfileCompletionPoints { get; set; } = 50;
    public int DailyLoginPoints { get; set; } = 1;
    public int WeeklyLoginBonusPoints { get; set; } = 10;
    public int MonthlyLoginBonusPoints { get; set; } = 50;
    public int ReferralPoints { get; set; } = 100;
    public int ReviewPoints { get; set; } = 15;
    public int GuideCreationPoints { get; set; } = 25;
    public int EventParticipationPoints { get; set; } = 20;
    public double PointsDecayRate { get; set; } = 0.0; // 0 = no decay
    public TimeSpan PointsDecayInterval { get; set; } = TimeSpan.FromDays(365);
    public int MaxDailyPoints { get; set; } = 500;
    public bool EnablePointsExpiration { get; set; } = false;
    public TimeSpan PointsExpirationPeriod { get; set; } = TimeSpan.FromDays(365);
}

/// <summary>
/// Badge system settings
/// </summary>
public class BadgeSettings
{
    public bool EnableBadges { get; set; } = true;
    public int MaxDisplayBadges { get; set; } = 5;
    public bool AllowBadgeCustomization { get; set; } = true;
    public bool ShowBadgeProgress { get; set; } = true;
    public bool EnableRareBadges { get; set; } = true;
    public bool EnableSeasonalBadges { get; set; } = true;
    public bool EnableCommunityBadges { get; set; } = true;
    public TimeSpan BadgeNotificationDelay { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableBadgeSharing { get; set; } = true;
    public bool EnableBadgeLeaderboard { get; set; } = true;
}

/// <summary>
/// Achievement system settings
/// </summary>
public class AchievementSettings
{
    public bool EnableAchievements { get; set; } = true;
    public bool ShowAchievementProgress { get; set; } = true;
    public bool EnableHiddenAchievements { get; set; } = true;
    public bool EnableProgressiveAchievements { get; set; } = true;
    public bool EnableTimeBasedAchievements { get; set; } = true;
    public bool EnableSocialAchievements { get; set; } = true;
    public bool EnableContentAchievements { get; set; } = true;
    public TimeSpan AchievementNotificationDelay { get; set; } = TimeSpan.FromMinutes(2);
    public bool EnableAchievementSharing { get; set; } = true;
    public int MaxAchievementRetries { get; set; } = 3;
}

/// <summary>
/// Leaderboard settings
/// </summary>
public class LeaderboardSettings
{
    public bool EnableLeaderboards { get; set; } = true;
    public bool EnableGlobalLeaderboard { get; set; } = true;
    public bool EnableWeeklyLeaderboard { get; set; } = true;
    public bool EnableMonthlyLeaderboard { get; set; } = true;
    public bool EnableCategoryLeaderboards { get; set; } = true;
    public int LeaderboardSize { get; set; } = 100;
    public int TopUsersDisplayCount { get; set; } = 10;
    public TimeSpan LeaderboardUpdateInterval { get; set; } = TimeSpan.FromHours(1);
    public bool EnableLeaderboardNotifications { get; set; } = true;
    public bool ShowUserRank { get; set; } = true;
    public bool EnableLeaderboardHistory { get; set; } = true;
    public TimeSpan LeaderboardHistoryRetention { get; set; } = TimeSpan.FromDays(365);
}

/// <summary>
/// Reward system settings
/// </summary>
public class RewardSettings
{
    public bool EnableRewards { get; set; } = true;
    public bool EnableVirtualRewards { get; set; } = true;
    public bool EnablePhysicalRewards { get; set; } = false;
    public bool EnableDiscountRewards { get; set; } = false;
    public bool EnableExperienceRewards { get; set; } = false;
    public int MinPointsForReward { get; set; } = 1000;
    public TimeSpan RewardClaimWindow { get; set; } = TimeSpan.FromDays(30);
    public bool EnableRewardNotifications { get; set; } = true;
    public bool EnableRewardHistory { get; set; } = true;
    public TimeSpan RewardHistoryRetention { get; set; } = TimeSpan.FromDays(365);
    public bool RequireRewardApproval { get; set; } = false;
    public int MaxRewardsPerUser { get; set; } = 10;
    public TimeSpan RewardCooldownPeriod { get; set; } = TimeSpan.FromDays(7);
}