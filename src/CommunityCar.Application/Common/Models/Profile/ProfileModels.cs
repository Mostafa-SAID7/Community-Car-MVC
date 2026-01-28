using System.ComponentModel.DataAnnotations;
using CommunityCar.Application.Common.Models.Identity;

namespace CommunityCar.Application.Common.Models.Profile;

#region Profile Requests

public class UpdatePrivacySettingsRequest
{
    public Guid UserId { get; set; }
    public bool ProfileVisible { get; set; } = true;
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
    public bool ShowOnlineStatus { get; set; } = true;
    public bool AllowTagging { get; set; } = true;
    public bool ShowActivityStatus { get; set; } = true;
}

public class UpdateNotificationSettingsRequest
{
    public Guid UserId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; }
    public bool MarketingEmails { get; set; }
}

public class UpdateProfileRequest
{
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? FirstName { get; set; }
    
    [StringLength(50)]
    public string? LastName { get; set; }
    
    [StringLength(500)]
    public string? Bio { get; set; }
    
    [StringLength(100)]
    public string? City { get; set; }
    
    public string? Country { get; set; }
    
    [StringLength(500)]
    public string? BioAr { get; set; }
    
    [StringLength(100)]
    public string? CityAr { get; set; }
    
    [StringLength(100)]
    public string? CountryAr { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
    
    public string? Website { get; set; }
    public string? Location { get; set; }
}

public class UploadImageRequest
{
    public Guid UserId { get; set; }
    public string ImageData { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? CaptionAr { get; set; }
    public bool IsProfilePicture { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
}

#endregion

#region Profile View Models

public class ProfileVM : UserSummaryVM
{
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? BioAr { get; set; }
    public string? CityAr { get; set; }
    public string? CountryAr { get; set; }
    public string? PhoneNumber { get; set; }
    public new string? ProfilePictureUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesReceived { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsOwnProfile { get; set; }
    public ProfileStatsVM? Stats { get; set; }
}

public class ProfileStatsVM
{
    public int PostsCount { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int LikesReceived { get; set; }
    public int CommentsReceived { get; set; }
    public int SharesReceived { get; set; }
    public int BadgesCount { get; set; }
    public int AchievementsCount { get; set; }
    public int Points { get; set; }
    public int TotalPoints { get; set; }
    public int ExperiencePoints { get; set; }
    public int Level { get; set; }
    public int Rank { get; set; }
}

public class ProfileSettingsVM : UserSummaryVM
{
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? BioAr { get; set; }
    public string? CityAr { get; set; }
    public string? CountryAr { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    public bool MarketingEmails { get; set; } = false;
}

public class PrivacySettingsVM
{
    public Guid UserId { get; set; }
    public bool ProfileVisible { get; set; } = true;
    public bool EmailVisible { get; set; }
    public bool PhoneVisible { get; set; }
    public bool ShowOnlineStatus { get; set; } = true;
    public bool AllowTagging { get; set; } = true;
    public bool ShowActivityStatus { get; set; } = true;
    public bool DataProcessingConsent { get; set; }
    public bool MarketingEmailsConsent { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class NotificationSettingsVM
{
    public Guid UserId { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool PushEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = false;
    public bool MarketingEnabled { get; set; } = false;
}

public class UserGalleryItemVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string? Caption { get; set; }
    public string? CaptionAr { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UploadedAt { get; set; }
    public int LikesCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentsCount { get; set; }
    public int ViewCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool IsFeatured { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

public class UserBadgeVM
{
    public Guid Id { get; set; }
    public string BadgeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Color { get; set; } = "#007bff";
    public DateTime EarnedAt { get; set; }
    public DateTime AwardedAt { get; set; }
    public bool IsRare { get; set; }
}

public class BadgeVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Color { get; set; } = "#007bff";
    public string Category { get; set; } = string.Empty;
    public int RequiredPoints { get; set; }
    public bool IsRare { get; set; }
    public bool IsEarned { get; set; }
}

public class UserAchievementVM
{
    public Guid Id { get; set; }
    public string AchievementId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime EarnedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int Progress { get; set; }
}

public class AchievementVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int Points { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Criteria { get; set; } = string.Empty;
    public bool IsEarned { get; set; }
    public DateTime? EarnedAt { get; set; }
}

public class UserGamificationStatsVM
{
    public int TotalPoints { get; set; }
    public int Level { get; set; }
    public int BadgesCount { get; set; }
    public int AchievementsCount { get; set; }
    public int Rank { get; set; }
    public int PointsToNextLevel { get; set; }
    public double LevelProgress { get; set; }
    public List<UserBadgeVM> RecentBadges { get; set; } = new();
    public List<UserAchievementVM> RecentAchievements { get; set; } = new();
}

public class PointTransactionVM
{
    public Guid Id { get; set; }
    public int Points { get; set; }
    public int Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime Timestamp { get; set; }
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
}

public class LeaderboardEntryVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public int Rank { get; set; }
    public int BadgesCount { get; set; }
    public int AchievementsCount { get; set; }
}

#endregion