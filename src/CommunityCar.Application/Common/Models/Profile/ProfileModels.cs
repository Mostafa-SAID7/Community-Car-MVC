using CommunityCar.Application.Common.Models.Identity;

namespace CommunityCar.Application.Common.Models.Profile;

#region Profile

public class ProfileVM : UserSummaryVM
{
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Website { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsVerified { get; set; }
    public bool IsEmailConfirmed { get; set; }
    
    // Stats
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesReceived { get; set; }
    public ProfileStatsVM? Stats { get; set; }
}

public class ProfileStatsVM
{
    public int Level { get; set; }
    public int ExperiencePoints { get; set; }
    public int TotalPoints { get; set; }
    public int Reputation { get; set; }
    public int CommunityContributionScore { get; set; }
    public int BadgesCount { get; set; }
    public int AchievementsCount { get; set; }
    public int Rank { get; set; }
}

public class UserGalleryItemVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public bool IsPublic { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPrimary { get; set; } // Legacy field
}

#endregion

#region Gamification

public class UserGamificationStatsVM
{
    public int Level { get; set; }
    public int Points { get; set; }
    public int Rank { get; set; }
    public List<UserBadgeVM> Badges { get; set; } = new();
}

public class UserBadgeVM : BadgeVM
{
    public Guid Id { get; set; }
    public DateTime AwardedAt { get; set; }
}

public class BadgeVM
{
    public string BadgeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UserAchievementVM : AchievementVM
{
    public Guid Id { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Progress { get; set; }
}

public class AchievementVM
{
    public string AchievementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int Points { get; set; }
}

public class LeaderboardEntryVM
{
    public int Rank { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
}

#endregion

#region Requests

public class UploadImageRequest
{
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string ImageData { get; set; } = string.Empty; // Base64
    public string? Caption { get; set; }
    public bool IsPublic { get; set; } = true;
}

public class UpdateProfileRequest
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}".Trim();
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Location => string.IsNullOrEmpty(City) ? Country : $"{City}, {Country}";
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
}

public class PointTransactionVM
{
    public int Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

#endregion
