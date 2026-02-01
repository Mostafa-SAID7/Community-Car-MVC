using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Groups.ViewModels;

public class GroupsSearchVM
{
    // Search parameters
    public string? SearchTerm { get; set; }
    public string? Query { get; set; }
    public string? Category { get; set; }
    public GroupPrivacy? Privacy { get; set; }
    public string? Location { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool? IsVerified { get; set; }
    public int? MinMembers { get; set; }
    public int? MaxMembers { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "MemberCount";
    public string SortOrder { get; set; } = "desc";
    
    // Results
    public List<GroupSummaryVM> Items { get; set; } = new();
    public List<GroupSummaryVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}