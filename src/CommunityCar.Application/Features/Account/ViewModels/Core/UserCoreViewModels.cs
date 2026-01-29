namespace CommunityCar.Application.Features.Account.ViewModels.Core;

public class ProfileVM
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? BioAr { get; set; }
    public string? CityAr { get; set; }
    public string? CountryAr { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastPasswordChangeAt { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool IsActive { get; set; }
    
    // OAuth connections
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    
    // Statistics
    public int PostsCount { get; set; }
    public int CommentsCount { get; set; }
    public int LikesReceived { get; set; }
    public int SharesReceived { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int AchievementsCount { get; set; }
    public int BadgesCount { get; set; }
}

public class ProfileStatsVM
{
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
}

/// <summary>
/// View model for user identity information
/// </summary>
public class UserIdentityVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsLocked { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();
}

/// <summary>
/// View model for role information
/// </summary>
public class RoleVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string ConcurrencyStamp { get; set; } = string.Empty;
}

/// <summary>
/// View model for user claim information
/// </summary>
public class UserClaimVM
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
}