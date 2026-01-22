using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Dashboard.DTOs;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Services.Dashboard;

public class DashboardManagementService : IDashboardManagementService
{
    private readonly ICurrentUserService _currentUserService;

    public DashboardManagementService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<List<UserManagementVM>> GetUserManagementHistoryAsync(int page = 1, int pageSize = 20)
    {
        // Mock data - in real implementation, query from database
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
        // In real implementation, perform the actual user action and log it
        await Task.CompletedTask;
        
        // Validate action
        var validActions = new[] { "Suspend", "Activate", "Delete", "Update", "Reset Password" };
        if (!validActions.Contains(request.Action))
        {
            return false;
        }

        // Log the action
        // In real implementation, save to database
        
        return true;
    }

    public async Task<bool> ReverseUserActionAsync(Guid actionId)
    {
        // In real implementation, reverse the user action if possible
        await Task.CompletedTask;
        return true;
    }

    public async Task ProcessExpiredActionsAsync()
    {
        // In real implementation, process expired temporary actions like suspensions
        await Task.CompletedTask;
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