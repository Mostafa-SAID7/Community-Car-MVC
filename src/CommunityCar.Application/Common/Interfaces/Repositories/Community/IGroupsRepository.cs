using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IGroupsRepository : IBaseRepository<Group>
{
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
}
