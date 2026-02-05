using CommunityCar.Application.Common.Models;
using CommunityCar.Application.Features.Dashboard.Management.Users.Actions;

namespace CommunityCar.Application.Features.Dashboard.Management.Users.Core;

/// <summary>
/// User management view model
/// </summary>
public class UserManagementVM
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
}

/// <summary>
/// Admin user management view model for dashboard operations
/// </summary>
public class AdminUserManagementVM
{
    public List<ManagedUserVM> Users { get; set; } = new();
    public AdminUserFiltersVM Filters { get; set; } = new();
    public AdminUserStatsVM Stats { get; set; } = new();
    public List<AdminUserManagementActionVM> AvailableActions { get; set; } = new();
    public PaginationInfo Pagination { get; set; } = new();
}

/// <summary>
/// Managed user view model for admin dashboard
/// </summary>
public class ManagedUserVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public bool IsVerified { get; set; }
}

/// <summary>
/// Admin user filters view model
/// </summary>
public class AdminUserFiltersVM
{
    public string? Search { get; set; }
    public string? Role { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsVerified { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public DateTime? LastLoginFrom { get; set; }
    public DateTime? LastLoginTo { get; set; }
}

/// <summary>
/// Admin user statistics view model
/// </summary>
public class AdminUserStatsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int LockedUsers { get; set; }
    public int VerifiedUsers { get; set; }
    public int UnverifiedUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
}