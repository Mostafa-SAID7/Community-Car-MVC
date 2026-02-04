namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class UserManagementVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int LoginCount { get; set; }
    public int FailedLoginAttempts { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public string? ProfilePicture { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? TimeZone { get; set; }
}

public class CreateUserVM
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public bool SendWelcomeEmail { get; set; } = true;
    public bool RequireEmailConfirmation { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? TimeZone { get; set; }
}

public class UpdateUserVM
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? TimeZone { get; set; }
}

public class BulkUserUpdateVM
{
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsTwoFactorEnabled { get; set; }
    public string? Status { get; set; }
    public DateTime? LockoutEnd { get; set; }
}

public class UserStatisticsVM
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int LockedUsers { get; set; }
    public int PendingUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int UsersWithTwoFactor { get; set; }
    public int UsersWithoutEmailConfirmation { get; set; }
    public decimal AverageLoginFrequency { get; set; }
    public string MostActiveUser { get; set; } = string.Empty;
    public decimal UserGrowthRate { get; set; }
}

public class UserActivityVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public bool Success { get; set; }
}