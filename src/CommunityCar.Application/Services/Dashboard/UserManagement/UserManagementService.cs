using CommunityCar.Application.Common.Interfaces.Services.Dashboard.UserManagement;
using CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Services.Dashboard.UserManagement;

public class UserManagementService : IUserManagementService
{
    public async Task<(List<UserManagementVM> Users, int TotalCount)> GetUsersAsync(string? search = null, string? role = null, bool? isActive = null, int page = 1, int pageSize = 20)
    {
        var users = new List<UserManagementVM>();
        var random = new Random();
        var roles = new[] { "Admin", "Moderator", "User", "Editor", "Viewer" };
        var statuses = new[] { "Active", "Inactive", "Locked", "Pending" };

        for (int i = 0; i < pageSize; i++)
        {
            var userId = Guid.NewGuid();
            var userNumber = random.Next(1, 10000);
            var isActiveUser = isActive ?? random.Next(10) > 1; // 90% active by default
            var userRole = role ?? roles[random.Next(roles.Length)];

            users.Add(new UserManagementVM
            {
                Id = userId,
                UserName = $"user{userNumber}",
                Email = $"user{userNumber}@example.com",
                FirstName = $"First{userNumber}",
                LastName = $"Last{userNumber}",
                Role = userRole,
                Status = isActiveUser ? "Active" : statuses[random.Next(1, statuses.Length)],
                IsActive = isActiveUser,
                IsEmailConfirmed = random.Next(10) > 1, // 90% confirmed
                IsTwoFactorEnabled = random.Next(3) == 0, // 33% have 2FA
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                LastLoginAt = isActiveUser ? DateTime.UtcNow.AddDays(-random.Next(0, 30)) : null,
                LoginCount = random.Next(1, 1000),
                FailedLoginAttempts = random.Next(0, 5),
                IsLocked = !isActiveUser && random.Next(3) == 0,
                LockoutEnd = null,
                ProfilePicture = null,
                PhoneNumber = $"+1-555-{random.Next(100, 999)}-{random.Next(1000, 9999)}",
                Country = new[] { "US", "UK", "CA", "AU", "DE" }[random.Next(5)],
                TimeZone = "UTC"
            });
        }

        var totalCount = random.Next(500, 5000);
        return await Task.FromResult((users, totalCount));
    }

    public async Task<UserManagementVM?> GetUserByIdAsync(Guid userId)
    {
        var random = new Random();
        
        return new UserManagementVM
        {
            Id = userId,
            UserName = "john.doe",
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "User",
            Status = "Active",
            IsActive = true,
            IsEmailConfirmed = true,
            IsTwoFactorEnabled = false,
            CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30, 365)),
            LastLoginAt = DateTime.UtcNow.AddHours(-random.Next(1, 24)),
            LoginCount = random.Next(50, 500),
            FailedLoginAttempts = 0,
            IsLocked = false,
            LockoutEnd = null,
            ProfilePicture = null,
            PhoneNumber = "+1-555-123-4567",
            Country = "US",
            TimeZone = "America/New_York"
        };
    }

    public async Task<bool> CreateUserAsync(CreateUserVM model)
    {
        // In real implementation, create user in database
        await Task.Delay(500);
        return true;
    }

    public async Task<bool> UpdateUserAsync(Guid userId, Features.Dashboard.UserManagement.ViewModels.UpdateUserVM model)
    {
        // In real implementation, update user in database
        await Task.Delay(300);
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        // In real implementation, soft delete user
        await Task.Delay(200);
        return true;
    }

    public async Task<bool> ActivateUserAsync(Guid userId)
    {
        // In real implementation, activate user account
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId)
    {
        // In real implementation, deactivate user account
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> LockUserAsync(Guid userId, string reason, DateTime? unlockDate = null)
    {
        // In real implementation, lock user account
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> UnlockUserAsync(Guid userId)
    {
        // In real implementation, unlock user account
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword)
    {
        // In real implementation, reset user password
        await Task.Delay(200);
        return true;
    }

    public async Task<bool> SendPasswordResetEmailAsync(Guid userId)
    {
        // In real implementation, send password reset email
        await Task.Delay(500);
        return true;
    }

    public async Task<List<string>> GetUserRolesAsync(Guid userId)
    {
        return await Task.FromResult(new List<string> { "User", "Member" });
    }

    public async Task<bool> AssignRoleAsync(Guid userId, string role)
    {
        // In real implementation, assign role to user
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> RemoveRoleAsync(Guid userId, string role)
    {
        // In real implementation, remove role from user
        await Task.Delay(100);
        return true;
    }

    public async Task<List<string>> GetAvailableRolesAsync()
    {
        return await Task.FromResult(new List<string>
        {
            "Admin", "Moderator", "Editor", "User", "Viewer", "Guest"
        });
    }

    public async Task<Features.Dashboard.UserManagement.ViewModels.UserStatisticsVM> GetUserStatisticsAsync()
    {
        var random = new Random();
        
        return new Features.Dashboard.UserManagement.ViewModels.UserStatisticsVM
        {
            TotalUsers = random.Next(1000, 10000),
            ActiveUsers = random.Next(800, 8000),
            InactiveUsers = random.Next(100, 1000),
            LockedUsers = random.Next(10, 100),
            PendingUsers = random.Next(5, 50),
            NewUsersToday = random.Next(5, 50),
            NewUsersThisWeek = random.Next(20, 200),
            NewUsersThisMonth = random.Next(100, 1000),
            UsersWithTwoFactor = random.Next(200, 2000),
            UsersWithoutEmailConfirmation = random.Next(50, 500),
            AverageLoginFrequency = (decimal)(random.NextDouble() * 10 + 1), // 1-11 times per week
            MostActiveUser = "admin@example.com",
            UserGrowthRate = (decimal)(random.NextDouble() * 20 + 5) // 5-25% growth
        };
    }

    public async Task<List<ChartDataVM>> GetUserRegistrationChartAsync(int days)
    {
        var data = new List<ChartDataVM>();
        var random = new Random();
        var startDate = DateTime.UtcNow.AddDays(-days);

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            data.Add(new ChartDataVM
            {
                Label = date.ToString("MMM dd"),
                Value = random.Next(5, 50), // New registrations per day
                Date = date
            });
        }

        return await Task.FromResult(data);
    }

    public async Task<List<Features.Dashboard.UserManagement.ViewModels.UserActivityVM>> GetRecentUserActivityAsync(int limit = 50)
    {
        var activities = new List<Features.Dashboard.UserManagement.ViewModels.UserActivityVM>();
        var random = new Random();
        var actions = new[] { "Login", "Logout", "Profile Update", "Password Change", "Post Created", "Comment Added" };
        var users = new[] { "john.doe", "jane.smith", "admin", "moderator", "editor" };

        for (int i = 0; i < limit; i++)
        {
            activities.Add(new Features.Dashboard.UserManagement.ViewModels.UserActivityVM
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = users[random.Next(users.Length)],
                Action = actions[random.Next(actions.Length)],
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                Success = random.Next(20) > 0 // 95% success rate
            });
        }

        return await Task.FromResult(activities.OrderByDescending(a => a.Timestamp).ToList());
    }

    public async Task<bool> BulkUpdateUsersAsync(List<Guid> userIds, BulkUserUpdateVM model)
    {
        // In real implementation, update multiple users
        await Task.Delay(1000);
        return true;
    }

    public async Task<bool> ExportUsersAsync(string format = "csv")
    {
        // In real implementation, export users to file
        await Task.Delay(2000);
        return true;
    }

    public async Task<bool> ImportUsersAsync(string filePath)
    {
        // In real implementation, import users from file
        await Task.Delay(3000);
        return true;
    }
}