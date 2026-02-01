using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Groups.ViewModels;

public class GroupVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? DescriptionAr { get; set; }
    public string? Category { get; set; }
    public string? CategoryAr { get; set; }
    public string? Rules { get; set; }
    public string? RulesAr { get; set; }
    public GroupPrivacy Privacy { get; set; }
    public bool RequiresApproval { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsOfficial { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? AvatarUrl { get; set; }
    public int MemberCount { get; set; }
    public int PostCount { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public string? Location { get; set; }
    public string? LocationAr { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Helper properties
    public string PrivacyText => Privacy.ToString();
    public string PrivacyIcon => Privacy switch
    {
        GroupPrivacy.Public => "globe",
        GroupPrivacy.Private => "lock",
        GroupPrivacy.Secret => "eye-off",
        _ => "globe"
    };
    
    public string ActivityText
    {
        get
        {
            if (!LastActivityAt.HasValue) return "No recent activity";
            var timeAgo = DateTime.UtcNow - LastActivityAt.Value;
            return timeAgo.TotalDays > 7 ? $"Active {(int)timeAgo.TotalDays} days ago" :
                   timeAgo.TotalHours > 1 ? $"Active {(int)timeAgo.TotalHours} hours ago" :
                   "Recently active";
        }
    }
    
    public string MemberCountText => MemberCount == 1 ? "1 member" : $"{MemberCount:N0} members";
    public string PostCountText => PostCount == 1 ? "1 post" : $"{PostCount:N0} posts";
}