using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class GroupsRepository : IGroupsRepository
{
    private readonly ApplicationDbContext _context;

    public GroupsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdAsync(Guid id)
    {
        return await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Group>> GetAllAsync()
    {
        return await _context.Groups.ToListAsync();
    }

    public async Task<(IEnumerable<Group> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        GroupPrivacy? privacy = null,
        string? category = null,
        string? sortBy = "newest",
        int page = 1,
        int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Groups.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(g => 
                g.Name.ToLower().Contains(search) ||
                g.Description.ToLower().Contains(search) ||
                (g.Category != null && g.Category.ToLower().Contains(search)));
        }

        if (privacy.HasValue)
        {
            query = query.Where(g => g.Privacy == privacy.Value);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(g => g.Category != null && g.Category.ToLower().Contains(category.ToLower()));
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "name" => query.OrderBy(g => g.Name),
            "members" => query.OrderByDescending(g => g.MemberCount),
            "activity" => query.OrderByDescending(g => g.LastActivityAt),
            "oldest" => query.OrderBy(g => g.CreatedAt),
            "newest" or _ => query.OrderByDescending(g => g.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Group>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Where(g => g.OwnerId == ownerId)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Group>> GetPopularGroupsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Where(g => g.Privacy == GroupPrivacy.Public)
            .OrderByDescending(g => g.MemberCount)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Group>> GetRecentlyActiveAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Where(g => g.Privacy == GroupPrivacy.Public)
            .OrderByDescending(g => g.LastActivityAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Group group)
    {
        await _context.Groups.AddAsync(group);
    }

    public Task UpdateAsync(Group group)
    {
        _context.Groups.Update(group);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Group group)
    {
        _context.Groups.Remove(group);
        return Task.CompletedTask;
    }
}
