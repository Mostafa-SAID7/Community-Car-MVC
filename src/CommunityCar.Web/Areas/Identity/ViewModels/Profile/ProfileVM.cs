using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Profile;

public class ProfileVM
{
    public Guid Id { get; set; }
    
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Slug { get; set; }
    
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
