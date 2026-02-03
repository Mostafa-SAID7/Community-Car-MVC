using CommunityCar.Application.Common.Interfaces.Repositories.Account;
using CommunityCar.Domain.Entities.Account.Management;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Account.Management;

/// <summary>
/// Repository implementation for UserManagement entity operations
/// </summary>
public class UserManagementRepository : BaseRepository<UserManagement>, IUserManagementRepository
{
    public UserManagementRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region User Management Operations

    public async Task<UserManagement?> GetUserManagementAsync(Guid userId)
    {
        return await Context.UserManagements
            .FirstOrDefaultAsync(um => um.UserId == userId);
    }

    public async Task<IEnumerable<UserManagement>> GetManagedUsersAsync(Guid managerId)
    {
        return await Context.UserManagements
            .Where(um => um.ManagerId == managerId)
            .OrderBy(um => um.AssignedAt)
            .ToListAsync();
    }

    public async Task<bool> AssignManagerAsync(Guid userId, Guid managerId)
    {
        var existingManagement = await GetUserManagementAsync(userId);
        
        if (existingManagement != null)
        {
            existingManagement.ChangeManager(managerId);
            await UpdateAsync(existingManagement);
        }
        else
        {
            var userManagement = UserManagement.Create(userId, managerId);
            await AddAsync(userManagement);
        }

        return true;
    }

    public async Task<bool> RemoveManagerAsync(Guid userId)
    {
        var userManagement = await GetUserManagementAsync(userId);
        
        if (userManagement == null) return false;

        await DeleteAsync(userManagement);
        return true;
    }

    public async Task<bool> TransferManagementAsync(Guid userId, Guid newManagerId)
    {
        var userManagement = await GetUserManagementAsync(userId);
        
        if (userManagement == null) return false;

        userManagement.ChangeManager(newManagerId);
        await UpdateAsync(userManagement);
        return true;
    }

    #endregion

    #region Management Hierarchy

    public async Task<IEnumerable<UserManagement>> GetManagementHierarchyAsync(Guid rootManagerId)
    {
        var hierarchy = new List<UserManagement>();
        var queue = new Queue<Guid>();
        queue.Enqueue(rootManagerId);

        while (queue.Count > 0)
        {
            var managerId = queue.Dequeue();
            var managedUsers = await GetManagedUsersAsync(managerId);
            
            hierarchy.AddRange(managedUsers);
            
            foreach (var managedUser in managedUsers)
            {
                queue.Enqueue(managedUser.UserId);
            }
        }

        return hierarchy;
    }

    public async Task<int> GetManagedUserCountAsync(Guid managerId)
    {
        return await Context.UserManagements
            .Where(um => um.ManagerId == managerId)
            .CountAsync();
    }

    public async Task<bool> IsManagerAsync(Guid userId)
    {
        return await Context.UserManagements
            .AnyAsync(um => um.ManagerId == userId);
    }

    public async Task<bool> CanManageUserAsync(Guid managerId, Guid userId)
    {
        // Direct management
        var directManagement = await Context.UserManagements
            .AnyAsync(um => um.ManagerId == managerId && um.UserId == userId);

        if (directManagement) return true;

        // Check hierarchy (indirect management)
        var hierarchy = await GetManagementHierarchyAsync(managerId);
        return hierarchy.Any(um => um.UserId == userId);
    }

    #endregion

    #region Management Analytics

    public async Task<Dictionary<Guid, int>> GetManagementStatisticsAsync()
    {
        return await Context.UserManagements
            .Where(um => um.ManagerId.HasValue)
            .GroupBy(um => um.ManagerId)
            .Select(g => new { ManagerId = g.Key!.Value, Count = g.Count() })
            .ToDictionaryAsync(x => x.ManagerId, x => x.Count);
    }

    public async Task<IEnumerable<UserManagement>> GetTopManagersAsync(int count = 10)
    {
        var topManagerIds = await Context.UserManagements
            .GroupBy(um => um.ManagerId)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();

        return await Context.UserManagements
            .Where(um => topManagerIds.Contains(um.ManagerId))
            .ToListAsync();
    }

    public async Task<IEnumerable<UserManagement>> GetRecentManagementChangesAsync(int count = 20)
    {
        return await Context.UserManagements
            .OrderByDescending(um => um.UpdatedAt)
            .Take(count)
            .ToListAsync();
    }

    #endregion
}