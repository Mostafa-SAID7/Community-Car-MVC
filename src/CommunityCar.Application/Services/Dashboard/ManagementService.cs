using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Services.Dashboard;

public class ManagementService : IManagementService
{
    private readonly ICurrentUserService _currentUserService;

    public ManagementService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<List<UserManagementVM>> GetUserManagementHistoryAsync(int page = 1, int pageSize = 20)
    {
        // Mock data
        var history = new List<UserManagementVM>();
        var actions = new[] { "Created", "Updated", "Suspended", "Activated", "Deleted" };
        var random = new Random();

        for (int i = 0; i < pageSize; i++)
        {
            var action = actions[random.Next(actions.Length)];
            var actionDate = DateTime.UtcNow.AddDays(-random.Next(1, 30));

            history.Add(new UserManagementVM
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = $"user{i + 1}@example.com",
                UserEmail = $"user{i + 1}@example.com",
                Action = action,
                Reason = GetReasonForAction(action),
                PerformedByName = "System Admin",
                ActionDate = actionDate,
                Notes = $"Action performed via dashboard",
                IsReversible = action != "Deleted",
                ExpiryDate = action == "Suspended" ? actionDate.AddDays(7) : null,
                IsExpired = false,
                ActionDateFormatted = actionDate.ToString("MMM dd, yyyy HH:mm")
            });
        }

        return await Task.FromResult(history.OrderByDescending(h => h.ActionDate).ToList());
    }

    public async Task<List<UserManagementVM>> GetUserManagementHistoryByUserAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        var allHistory = await GetUserManagementHistoryAsync(1, 100);
        return allHistory.Where(h => h.UserId == userId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
    }

    public async Task<bool> PerformUserActionAsync(UserManagementRequest request)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ReverseUserActionAsync(Guid actionId)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task ProcessExpiredActionsAsync()
    {
        await Task.CompletedTask;
    }

    public async Task<List<SystemHealthVM>> GetSystemHealthAsync()
    {
        return new List<SystemHealthVM>
        {
            new()
            {
                ServiceName = "Web Application",
                Status = "Healthy",
                IsHealthy = true,
                ResponseTime = 150,
                CpuUsage = 25,
                MemoryUsage = 40,
                DiskUsage = 30,
                ActiveConnections = 1500,
                LastCheck = DateTime.UtcNow
            },
            new()
            {
                ServiceName = "Database",
                Status = "Healthy",
                IsHealthy = true,
                ResponseTime = 20,
                CpuUsage = 15,
                MemoryUsage = 60,
                DiskUsage = 45,
                ActiveConnections = 50,
                LastCheck = DateTime.UtcNow
            }
        };
    }

    public async Task<List<UserManagementVM>> GetUsersAsync(int page, int pageSize, string? search)
    {
        var users = new List<UserManagementVM>();
        for (int i = 0; i < pageSize; i++)
        {
            users.Add(new UserManagementVM
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = $"user{i + 1}",
                Email = $"user{i + 1}@example.com",
                Role = i % 10 == 0 ? "Admin" : "User",
                IsActive = true,
                IsBlocked = false,
                LastLogin = DateTime.UtcNow.AddMinutes(-new Random().Next(1, 10000)),
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            });
        }
        return await Task.FromResult(users);
    }

    public async Task<UserManagementVM?> GetUserAsync(Guid userId)
    {
        return new UserManagementVM
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = "sample_user",
            Email = "user@example.com",
            Role = "User",
            IsActive = true,
            IsBlocked = false,
            LastLogin = DateTime.UtcNow.AddHours(-2),
            CreatedAt = DateTime.UtcNow.AddMonths(-3)
        };
    }

    public async Task<bool> BlockUserAsync(Guid userId, string reason)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> UnblockUserAsync(Guid userId)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> UpdateUserRoleAsync(Guid userId, string role)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<List<ContentModerationVM>> GetContentModerationAsync(int page, int pageSize)
    {
        return new List<ContentModerationVM>
        {
            new()
            {
                TotalCount = 50,
                PendingCount = 10,
                ApprovedCount = 30,
                RejectedCount = 10,
                Items = new List<ModerationItemVM>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Type = "Post",
                        Title = "Sample Post",
                        Content = "This is a sample post for moderation",
                        AuthorName = "User1",
                        CreatedAt = DateTime.UtcNow.AddHours(-5),
                        Status = "Pending",
                        ReportCount = 2,
                        ReportReasons = new List<string> { "Spam", "Inappropriate" }
                    }
                }
            }
        };
    }

    public async Task<bool> ModerateContentAsync(ModerateContentRequest request)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<List<SystemAlertVM>> GetSystemAlertsAsync()
    {
        return new List<SystemAlertVM>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Type = "Security",
                Message = "Multiple failed login attempts detected",
                Severity = "High",
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                IsRead = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Type = "Performance",
                Message = "High CPU usage on Database server",
                Severity = "Medium",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                IsRead = false
            }
        };
    }

    private string GetReasonForAction(string action)
    {
        return action switch
        {
            "Created" => "New user registration",
            "Updated" => "Profile information updated",
            "Suspended" => "Violation of community guidelines",
            "Activated" => "Account reactivated after review",
            "Deleted" => "Account deletion requested by user",
            _ => "System action"
        };
    }
}