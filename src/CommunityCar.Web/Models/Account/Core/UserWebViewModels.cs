namespace CommunityCar.Web.Models.Account.Core;

public class UserDashboardWebVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int PostsCount { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int AchievementsCount { get; set; }
    public int BadgesCount { get; set; }
    public DateTime LastLoginAt { get; set; }
    public List<string> RecentActivities { get; set; } = new();
}

public class UserProfileWebVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePicture { get; set; }
    public string? CoverPhoto { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneConfirmed { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}