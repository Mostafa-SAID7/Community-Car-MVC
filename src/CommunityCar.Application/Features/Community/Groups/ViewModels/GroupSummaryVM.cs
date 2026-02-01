using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Groups.ViewModels;

public class GroupSummaryVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string? Category { get; set; }
    public string? CategoryAr { get; set; }
    public string? Location { get; set; }
    public string? LocationAr { get; set; }
    public GroupPrivacy Privacy { get; set; }
    public int MemberCount { get; set; }
    public int PostCount { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime? LastActivityAt { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOfficial { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string MemberCountText => MemberCount == 1 ? "1 member" : $"{MemberCount:N0} members";
}