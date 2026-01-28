using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IGroupsRepository
{
    Task<Group?> GetByIdAsync(Guid id);
    Task<IEnumerable<Group>> GetAllAsync();
    Task<(IEnumerable<Group> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        GroupPrivacy? privacy = null,
        string? category = null,
        string? sortBy = "newest",
        int page = 1,
        int pageSize = 12,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetPopularGroupsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetRecentlyActiveAsync(int count = 10, CancellationToken cancellationToken = default);
    Task AddAsync(Group group);
    Task UpdateAsync(Group group);
    Task DeleteAsync(Group group);
}


