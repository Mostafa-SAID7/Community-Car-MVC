using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Groups.ViewModels;

public class GroupsStatsVM
{
    public int TotalGroups { get; set; }
    public int PublicGroups { get; set; }
    public int PrivateGroups { get; set; }
    public int TotalMembers { get; set; }
    public List<GroupSummaryVM> PopularGroups { get; set; } = new();
    public List<GroupSummaryVM> RecentlyActiveGroups { get; set; } = new();
    public Dictionary<string, int> GroupsByCategory { get; set; } = new();
}