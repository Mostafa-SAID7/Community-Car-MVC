using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Entities.Account.Core;
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

    public async Task<Group?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await Context.Groups
            .FirstOrDefaultAsync(g => g.Slug == slug, cancellationToken);
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

    // BroadcastHub specific methods
    public async Task<bool> UserHasAccessAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await Context.Groups.FindAsync(new object[] { groupId }, cancellationToken);
        if (group == null) return false;

        // Public groups are accessible to everyone
        if (group.Privacy == GroupPrivacy.Public) return true;

        // Check if user is owner
        return group.OwnerId == userId;
    }

    public async Task<bool> UserCanPostAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await Context.Groups.FindAsync(new object[] { groupId }, cancellationToken);
        if (group == null) return false;

        // Owner can always post
        if (group.OwnerId == userId) return true;

        // For now, assume public groups allow posting by anyone
        return group.Privacy == GroupPrivacy.Public;
    }

    public async Task<IEnumerable<Group>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Groups
            .Where(g => g.OwnerId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAdminsAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await Context.Groups.FindAsync(new object[] { groupId }, cancellationToken);
        if (group == null) return new List<User>();

        // Get the owner user
        var owner = await Context.Users.FindAsync(new object[] { group.OwnerId }, cancellationToken);
        if (owner == null) return new List<User>();

        // For now, only return the owner as admin
        return new List<User> { owner };
    }

    public async Task AddMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        // For now, just a placeholder - would need to implement member management
        await Task.CompletedTask;
    }

    public async Task CreateJoinRequestAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        // For now, just a placeholder - would need to implement join request entity
        await Task.CompletedTask;
    }

    public async Task<bool> IsUserAdminAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await Context.Groups.FindAsync(new object[] { groupId }, cancellationToken);
        if (group == null) return false;

        // Check if user is owner (admin)
        return group.OwnerId == userId;
    }

    public async Task ApproveJoinRequestAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        // For now, just a placeholder - would need to implement join request approval
        await Task.CompletedTask;
    }

    public async Task DenyJoinRequestAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        // For now, just a placeholder - would need to implement join request denial
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<dynamic>> GetUserGroupsWithAccessLevelAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var groups = await Context.Groups
            .Where(g => g.OwnerId == userId)
            .Select(g => new
            {
                g.Id,
                g.Name,
                UserAccessLevel = "Admin",
                IsUserAdmin = true,
                CanUserPost = true,
                CanUserModerate = true
            })
            .ToListAsync(cancellationToken);

        return groups.Cast<dynamic>();
    }
}
