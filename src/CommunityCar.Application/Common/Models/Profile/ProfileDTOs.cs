using CommunityCar.Application.Common.Models.Identity;

namespace CommunityCar.Application.Common.Models.Profile;

#region Profile DTOs

public class ProfileDTO : UserSummaryVM
{
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? BioAr { get; set; }
    public string? CityAr { get; set; }
    public string? CountryAr { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostsCount { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsOwnProfile { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class ProfileSettingsDTO : UserSummaryVM
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
}

public class ProfileStatsDTO
{
    public int PostsCount { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int LikesReceived { get; set; }
    public int CommentsReceived { get; set; }
    public int SharesReceived { get; set; }
    public int ViewsReceived { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public int BadgesCount { get; set; }
    public int AchievementsCount { get; set; }
}

#endregion