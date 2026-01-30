using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CommunityCar.Application.Features.Account.ViewModels.Core;

public class ProfileVM
{
    public Guid Id { get; set; }
    
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    public string? UserName { get; set; }
    
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
    
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    
    [Display(Name = "Bio (Arabic)")]
    public string? BioAr { get; set; }
    
    [Display(Name = "City (Arabic)")]
    public string? CityAr { get; set; }
    
    [Display(Name = "Country (Arabic)")]
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
    
    // UI specific
    public bool IsFollowing { get; set; }
    public bool IsFollowedBy { get; set; }
    public bool CanFollow { get; set; } = true;
    public bool IsOwnProfile { get; set; }
    
    // View compatibility aliases
    public int LikesReceivedCount => LikesReceived;
    public int FriendsCount => FollowersCount; // Treating followers as friends for now
    
    public ProfileStatsVM Stats { get; set; } = new();
}

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

public class ProfileStatsVM : UserStatisticsVM { }

public class ProfileSettingsVM
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    [Display(Name = "Bio")]
    public string? Bio { get; set; }

    [StringLength(100)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [Display(Name = "Country")]
    public string? Country { get; set; }

    [StringLength(500)]
    [Display(Name = "Bio (Arabic)")]
    public string? BioAr { get; set; }

    [StringLength(100)]
    [Display(Name = "City (Arabic)")]
    public string? CityAr { get; set; }

    [StringLength(100)]
    [Display(Name = "Country (Arabic)")]
    public string? CountryAr { get; set; }

    [Display(Name = "Profile Picture")]
    public IFormFile? ProfilePicture { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    
    // Notification and Privacy flags (flattened)
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = true;
    public bool MarketingEmails { get; set; } = true;
    public bool PublicProfile { get; set; } = true;
}

public class UpdateProfileVM
{
    public Guid UserId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Bio")]
    public string? Bio { get; set; }

    [StringLength(100)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [Display(Name = "Country")]
    public string? Country { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? Website { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}

public class UserCardVM
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsFollowing { get; set; }
    public int FollowersCount { get; set; }
    public int PostsCount { get; set; }
}

public class AccountIdentityVM
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

public class AccountClaimVM
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
}

// Aliases for compatibility
public class ProfileIndexVM : ProfileVM { }
public class UpdateProfileRequest : UpdateProfileVM { }
public class UserIdentityVM : AccountIdentityVM { }
public class UserClaimVM : AccountClaimVM { }