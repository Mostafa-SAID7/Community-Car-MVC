using System.ComponentModel.DataAnnotations;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Groups.DTOs;

public class CreateGroupRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Category { get; set; }
    
    [StringLength(2000)]
    public string? Rules { get; set; }
    
    public GroupPrivacy Privacy { get; set; } = GroupPrivacy.Public;
    
    public bool RequiresApproval { get; set; } = false;
    
    [StringLength(200)]
    public string? Location { get; set; }
    
    [StringLength(100)]
    public string? NameAr { get; set; }
    
    [StringLength(1000)]
    public string? DescriptionAr { get; set; }
    
    [StringLength(50)]
    public string? CategoryAr { get; set; }
    
    [StringLength(2000)]
    public string? RulesAr { get; set; }
    
    [StringLength(200)]
    public string? LocationAr { get; set; }
    
    public List<string>? Tags { get; set; }
    
    [Url]
    public string? CoverImageUrl { get; set; }
    
    [Url]
    public string? AvatarUrl { get; set; }
}

public class UpdateGroupRequest
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Category { get; set; }
    
    [StringLength(2000)]
    public string? Rules { get; set; }
    
    public GroupPrivacy Privacy { get; set; } = GroupPrivacy.Public;
    
    public bool RequiresApproval { get; set; } = false;
    
    [StringLength(200)]
    public string? Location { get; set; }
    
    [StringLength(100)]
    public string? NameAr { get; set; }
    
    [StringLength(1000)]
    public string? DescriptionAr { get; set; }
    
    [StringLength(50)]
    public string? CategoryAr { get; set; }
    
    [StringLength(2000)]
    public string? RulesAr { get; set; }
    
    [StringLength(200)]
    public string? LocationAr { get; set; }
    
    public List<string>? Tags { get; set; }
    
    [Url]
    public string? CoverImageUrl { get; set; }
    
    [Url]
    public string? AvatarUrl { get; set; }
}

public class GroupsSearchRequest
{
    public string? SearchTerm { get; set; }
    public GroupPrivacy? Privacy { get; set; }
    public string? Category { get; set; }
    public string? SortBy { get; set; } = "newest"; // newest, oldest, name, members, activity
    
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 12;
}

public class GroupsSearchResponse
{
    public IEnumerable<GroupSummaryVM> Items { get; set; } = new List<GroupSummaryVM>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public class GroupSummaryVM
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
    
    // Helper properties
    public string PrivacyText => Privacy.ToString();
    public string ActivityText
    {
        get
        {
            if (!LastActivityAt.HasValue) return "No activity";
            var timeAgo = DateTime.UtcNow - LastActivityAt.Value;
            return timeAgo.TotalDays > 7 ? $"{(int)timeAgo.TotalDays} days ago" :
                   timeAgo.TotalHours > 1 ? $"{(int)timeAgo.TotalHours} hours ago" :
                   "Recently active";
        }
    }
}


