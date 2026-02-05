using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management;
using CommunityCar.Domain.Entities.Account.Management;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Dashboard.Management;

/// <summary>
/// Repository implementation for UserManagementAction entity operations (Dashboard/Admin context)
/// </summary>
public class UserManagementActionRepository : BaseRepository<UserManagementAction>, IUserManagementActionRepository
{
    public UserManagementActionRepository(ApplicationDbContext context) : base(context)
    {
    }

    #region Management Action Tracking

    public async Task<IEnumerable<UserManagementAction>> GetUserActionsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.UserManagementActions
            .Where(uma => uma.TargetUserId == userId)
            .OrderByDescending(uma => uma.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserManagementAction>> GetManagerActionsAsync(Guid managerId, int page = 1, int pageSize = 20)
    {
        return await Context.UserManagementActions
            .Where(uma => uma.ManagerId == managerId)
            .OrderByDescending(uma => uma.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserManagementAction>> GetActionsByTypeAsync(string actionType, int page = 1, int pageSize = 20)
    {
        return await Context.UserManagementActions
            .Where(uma => uma.ActionType == actionType)
            .OrderByDescending(uma => uma.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task LogManagementActionAsync(Guid managerId, Guid targetUserId, string actionType, string description, Dictionary<string, object>? metadata = null)
    {
        var action = UserManagementAction.Create(targetUserId, managerId, actionType, description);
        await AddAsync(action);
    }

    #endregion

    #region Action Analytics

    public async Task<int> GetActionCountAsync(Guid? managerId = null, Guid? targetUserId = null, DateTime? fromDate = null)
    {
        var query = Context.UserManagementActions.AsQueryable();

        if (managerId.HasValue)
        {
            query = query.Where(uma => uma.ManagerId == managerId.Value);
        }

        if (targetUserId.HasValue)
        {
            query = query.Where(uma => uma.TargetUserId == targetUserId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(uma => uma.CreatedAt >= fromDate.Value);
        }

        return await query.CountAsync();
    }

    public async Task<Dictionary<string, int>> GetActionCountByTypeAsync(Guid? managerId = null, DateTime? fromDate = null)
    {
        var query = Context.UserManagementActions.AsQueryable();

        if (managerId.HasValue)
        {
            query = query.Where(uma => uma.ManagerId == managerId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(uma => uma.CreatedAt >= fromDate.Value);
        }

        var stats = await query
            .GroupBy(uma => uma.ActionType)
            .Select(g => new { ActionType = g.Key, Count = g.Count() })
            .ToListAsync();

        return stats.ToDictionary(x => x.ActionType, x => x.Count);
    }

    public async Task<IEnumerable<UserManagementAction>> GetRecentActionsAsync(int count = 20)
    {
        return await Context.UserManagementActions
            .OrderByDescending(uma => uma.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<DateTime?> GetLastActionDateAsync(Guid managerId)
    {
        var lastAction = await Context.UserManagementActions
            .Where(uma => uma.ManagerId == managerId)
            .OrderByDescending(uma => uma.CreatedAt)
            .FirstOrDefaultAsync();

        return lastAction?.CreatedAt;
    }

    #endregion

    #region Action Audit

    public async Task<IEnumerable<UserManagementAction>> GetAuditTrailAsync(Guid userId, DateTime? fromDate = null)
    {
        var query = Context.UserManagementActions
            .Where(uma => uma.TargetUserId == userId || uma.ManagerId == userId);

        if (fromDate.HasValue)
        {
            query = query.Where(uma => uma.CreatedAt >= fromDate.Value);
        }

        return await query
            .OrderByDescending(uma => uma.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> DeleteOldActionsAsync(DateTime cutoffDate)
    {
        var oldActions = await Context.UserManagementActions
            .Where(uma => uma.CreatedAt < cutoffDate)
            .ToListAsync();

        if (oldActions.Any())
        {
            Context.UserManagementActions.RemoveRange(oldActions);
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<UserManagementAction>> SearchActionsAsync(string searchTerm, int page = 1, int pageSize = 20)
    {
        return await Context.UserManagementActions
            .Where(uma => 
                uma.ActionType.Contains(searchTerm) ||
                uma.Description.Contains(searchTerm) ||
                uma.Reason != null && uma.Reason.Contains(searchTerm))
            .OrderByDescending(uma => uma.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    #endregion
}