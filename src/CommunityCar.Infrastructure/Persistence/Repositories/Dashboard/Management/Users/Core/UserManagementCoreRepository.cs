using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Core;
using CommunityCar.Application.Features.Dashboard.Management.Users.Core;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Dashboard.Management.Users.Core;

public class UserManagementCoreRepository : IUserManagementRepository
{
    private readonly ApplicationDbContext _context;

    public UserManagementCoreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<UserManagementDashboardVM> Users, int TotalCount)> GetUsersAsync(
        string? search = null, 
        string? role = null, 
        bool? isActive = null, 
        int page = 1, 
        int pageSize = 20)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.UserName!.Contains(search) || u.Email!.Contains(search));
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => !u.IsDeleted == isActive.Value);
        }

        var totalCount = await query.CountAsync();
        
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserManagementDashboardVM
            {
                UserStats = new AdminUserStatsVM(),
                SecurityStats = new CommunityCar.Application.Features.Dashboard.Reports.Users.Security.SecurityStatsVM(),
                Summary = new UserManagementSummaryVM(),
                QuickActions = new List<QuickActionVM>(),
                RecentUserActivity = new List<RecentUserActivityVM>(),
                RecentSecurityLogs = new List<CommunityCar.Application.Features.Dashboard.Management.Users.Security.UserSecurityLogVM>(),
                UserGrowthChart = new List<CommunityCar.Application.Features.Shared.ViewModels.ChartDataVM>(),
                LoginActivityChart = new List<CommunityCar.Application.Features.Shared.ViewModels.ChartDataVM>()
            })
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<UserManagementVM?> GetUserManagementByIdAsync(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return null;

        return new UserManagementVM
        {
            Id = user.Id,
            UserName = user.UserName ?? "",
            Email = user.Email ?? "",
            IsActive = !user.IsDeleted,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<string> CreateUserAsync(AdminCreateUserVM model)
    {
        // Implementation for creating user
        return await Task.FromResult(Guid.NewGuid().ToString());
    }

    public async Task<bool> UpdateUserAsync(DashboardUpdateUserVM model)
    {
        // Implementation for updating user
        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteUserAsync(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreUserAsync(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        user.IsDeleted = false;
        user.DeletedAt = null;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserManagementStatsVM> GetUserStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var activeUsers = await _context.Users.CountAsync(u => !u.IsDeleted);
        var newUsersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30));

        return new UserManagementStatsVM
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = totalUsers - activeUsers,
            NewUsersThisMonth = newUsersThisMonth,
            UsersByRole = new Dictionary<string, int>()
        };
    }

    public async Task<List<UserManagementDashboardVM>> GetRecentUsersAsync(int count = 10)
    {
        return await Task.FromResult(new List<UserManagementDashboardVM>());
    }
}