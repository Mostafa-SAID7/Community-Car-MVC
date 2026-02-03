using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IGroupsRepository : IBaseRepository<Group>
{
    Task<Group?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Group> Items, int TotalCount)> SearchAsync(
        string? searchTerm, 
        GroupPrivacy? privacy, 
        string? category, 
        string? sortBy, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetByOwnerAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetPopularGroupsAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetRecentlyActiveAsync(int count, CancellationToken cancellationToken = default);
    
    // BroadcastHub specific methods
    Task<bool> UserHasAccessAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);
    Task<bool> UserCanPostAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAdminsAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task AddMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);
    Task CreateJoinRequestAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserAdminAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);
    Task ApproveJoinRequestAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);
    Task DenyJoinRequestAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<dynamic>> GetUserGroupsWithAccessLevelAsync(Guid userId, CancellationToken cancellationToken = default);
}
