using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class GroupsRepository : BaseRepository<Group>, IGroupsRepository
{
    public GroupsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Group> Items, int TotalCount)> SearchAsync(
        string? searchTerm, 
        GroupPrivacy? privacy, 
        string? category, 
        string? sortBy, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = Context.Groups.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(g => g.Name.Contains(searchTerm));

        if (privacy.HasValue)
            query = query.Where(g => g.Privacy == privacy.Value);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(g => g.Category == category);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Group>> GetByOwnerAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Groups
            .Where(g => g.OwnerId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Group>> GetPopularGroupsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await Context.Groups
            .OrderByDescending(g => g.MemberCount)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Group>> GetRecentlyActiveAsync(int count, CancellationToken cancellationToken = default)
    {
        return await Context.Groups
            .OrderByDescending(g => g.UpdatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
