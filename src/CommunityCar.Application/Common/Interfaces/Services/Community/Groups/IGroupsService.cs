using CommunityCar.Application.Features.Community.Groups.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.Groups;

public interface IGroupsService
{
    Task<GroupVM?> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GroupVM?> GetGroupBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<GroupsSearchVM> SearchGroupsAsync(GroupsSearchVM request, CancellationToken cancellationToken = default);
    Task<GroupVM> CreateGroupAsync(CreateGroupVM request, CancellationToken cancellationToken = default);
    Task<GroupVM> UpdateGroupAsync(Guid id, UpdateGroupVM request, CancellationToken cancellationToken = default);
    Task<bool> DeleteGroupAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupVM>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupVM>> GetPopularGroupsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupVM>> GetRecentlyActiveGroupsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<GroupsStatsVM> GetGroupsStatsAsync(CancellationToken cancellationToken = default);
    Task<bool> JoinGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<bool> LeaveGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
}