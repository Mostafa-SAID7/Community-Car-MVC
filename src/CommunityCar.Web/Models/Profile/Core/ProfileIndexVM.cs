using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Profile.Core;

public class ProfileIndexVM
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? BioAr { get; set; }
    public string? CityAr { get; set; }
    public string? CountryAr { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
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
    public int FriendsCount { get; set; }
    public int LikesReceivedCount { get; set; }
    public int BadgesCount { get; set; }
    
    // Following/Followers
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsFollowedBy { get; set; }
    public bool CanFollow { get; set; } = true;
    public bool IsOwnProfile { get; set; }
}