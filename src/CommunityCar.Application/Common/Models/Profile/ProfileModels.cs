namespace CommunityCar.Application.Common.Models.Profile;

#region Profile

public class ProfileVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Website { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public bool IsEmailConfirmed { get; set; }
    
    // Stats
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostsCount { get; set; }
    public ProfileStatsVM? Stats { get; set; }
}

public class ProfileStatsVM
{
    public int Reputation { get; set; }
    public int Level { get; set; }
    public int ExperiencePoints { get; set; }
    public int CommunityContributionScore { get; set; }
}

public class UserGalleryItemVM
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsPrimary { get; set; }
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

public class UserBadgeVM
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; }
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


