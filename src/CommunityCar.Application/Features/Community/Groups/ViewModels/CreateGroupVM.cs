using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Groups.ViewModels;

public class CreateGroupVM
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? Category { get; set; }
    public string? CategoryAr { get; set; }
    public string? Rules { get; set; }
    public string? RulesAr { get; set; }
    public GroupPrivacy Privacy { get; set; } = GroupPrivacy.Public;
    public bool RequiresApproval { get; set; }
    public string? Location { get; set; }
    public string? LocationAr { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string> Tags { get; set; } = new();
}