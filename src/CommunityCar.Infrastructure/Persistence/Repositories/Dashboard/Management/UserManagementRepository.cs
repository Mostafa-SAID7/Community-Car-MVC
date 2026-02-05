using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Management.Users.Core;
using CommunityCar.Application.Features.Dashboard.Management.Users.Core;
using CommunityCar.Domain.Entities.Account.Core;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Dashboard.Management;

public class UserManagementRepository : IUserManagementRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public UserManagementRepository(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
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
            query = query.Where(u => u.UserName!.Contains(search) || 
                                   u.Email!.Contains(search) || 
                                   u.FirstName.Contains(search) || 
                                   u.LastName.Contains(search));
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
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
        return await _context.Users.FindAsync(userId);
    }

    public async Task<UserManagementVM?> GetUserManagementByIdAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new UserManagementVM
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }

    public async Task<string> CreateUserAsync(AdminCreateUserVM model)
    {
        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        return result.Succeeded ? user.Id : string.Empty;
    }

    public async Task<bool> UpdateUserAsync(DashboardUpdateUserVM model)
    {
        var user = await _context.Users.FindAsync(model.Id);
        if (user == null) return false;

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.IsActive = model.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsDeleted = false;
        user.DeletedAt = null;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserManagementStatsVM> GetUserStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
        var today = DateTime.UtcNow.Date;
        var thisWeek = DateTime.UtcNow.AddDays(-7);
        var thisMonth = DateTime.UtcNow.AddDays(-30);

        return new UserManagementStatsVM
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            NewUsersToday = await _context.Users.CountAsync(u => u.CreatedAt >= today),
            NewUsersThisWeek = await _context.Users.CountAsync(u => u.CreatedAt >= thisWeek),
            NewUsersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= thisMonth),
            UsersByRole = new Dictionary<string, int>() // Would need role data
        };
    }

    public async Task<List<UserManagementDashboardVM>> GetRecentUsersAsync(int count = 10)
    {
        return await Task.FromResult(new List<UserManagementDashboardVM>());
    }
}