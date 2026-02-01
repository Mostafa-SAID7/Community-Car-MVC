using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Groups.ViewModels;

public class GroupsIndexVM
{
    public GroupsSearchVM SearchRequest { get; set; } = new();
    public GroupsSearchVM SearchResponse { get; set; } = new();
    public GroupsStatsVM Stats { get; set; } = new();
}