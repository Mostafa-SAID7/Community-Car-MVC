using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Audit;
using CommunityCar.Application.Features.Dashboard.Audit.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Services.Dashboard.Audit;

public class AuditService : IAuditService
{
    public async Task<List<AuditLogVM>> GetAuditLogsAsync(DateTime? startDate = null, DateTime? endDate = null, string? userId = null, string? action = null, int page = 1, int pageSize = 50)
    {
        var logs = new List<AuditLogVM>();
        var random = new Random();
        var actions = new[] { "Create", "Update", "Delete", "Login", "Logout", "View", "Export", "Import" };
        var entityTypes = new[] { "User", "Post", "Comment", "Group", "Event", "News", "Review", "Story" };
        var users = new[] { "admin", "user1", "user2", "moderator", "editor" };

        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        for (int i = 0; i < Math.Min(pageSize, 100); i++)
        {
            var timestamp = start.AddMinutes(random.Next(0, (int)(end - start).TotalMinutes));
            var selectedAction = action ?? actions[random.Next(actions.Length)];
            var selectedUser = userId ?? users[random.Next(users.Length)];

            logs.Add(new AuditLogVM
            {
                Id = Guid.NewGuid(),
                UserId = selectedUser,
                UserName = $"{selectedUser}@example.com",
                Action = selectedAction,
                EntityType = entityTypes[random.Next(entityTypes.Length)],
                EntityId = Guid.NewGuid().ToString(),
                Timestamp = timestamp,
                IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                OldValues = selectedAction == "Update" ? "{\"name\":\"Old Value\"}" : null,
                NewValues = selectedAction is "Create" or "Update" ? "{\"name\":\"New Value\"}" : null,
                Success = random.Next(20) > 0, // 95% success rate
                ErrorMessage = random.Next(20) == 0 ? "Operation failed" : null
            });
        }

        return await Task.FromResult(logs.OrderByDescending(l => l.Timestamp).ToList());
    }

    public async Task<AuditLogVM?> GetAuditLogByIdAsync(Guid id)
    {
        var random = new Random();
        
        return new AuditLogVM
        {
            Id = id,
            UserId = "user1",
            UserName = "user1@example.com",
            Action = "Update",
            EntityType = "User",
            EntityId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
            IpAddress = "192.168.1.100",
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
            OldValues = "{\"name\":\"John Doe\",\"email\":\"john@example.com\"}",
            NewValues = "{\"name\":\"John Smith\",\"email\":\"john.smith@example.com\"}",
            Success = true,
            ErrorMessage = null
        };
    }

    public async Task LogActionAsync(string userId, string action, string entityType, string entityId, string? oldValues = null, string? newValues = null, string? ipAddress = null)
    {
        // In real implementation, save to database
        await Task.Delay(10);
    }

    public async Task<List<string>> GetAuditActionsAsync()
    {
        return await Task.FromResult(new List<string>
        {
            "Create", "Update", "Delete", "Login", "Logout", "View", "Export", "Import", 
            "Approve", "Reject", "Publish", "Unpublish", "Archive", "Restore"
        });
    }

    public async Task<List<string>> GetAuditEntityTypesAsync()
    {
        return await Task.FromResult(new List<string>
        {
            "User", "Post", "Comment", "Group", "Event", "News", "Review", "Story", 
            "Guide", "Question", "Answer", "Category", "Tag", "Setting"
        });
    }

    public async Task<AuditStatisticsVM> GetAuditStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var random = new Random();
        
        return new AuditStatisticsVM
        {
            TotalActions = random.Next(1000, 10000),
            UniqueUsers = random.Next(50, 500),
            SuccessfulActions = random.Next(900, 9500),
            FailedActions = random.Next(10, 500),
            MostActiveUser = "admin@example.com",
            MostCommonAction = "View",
            MostAffectedEntityType = "Post",
            AverageActionsPerDay = random.Next(50, 500),
            PeakActivityHour = random.Next(9, 17), // 9 AM to 5 PM
            TopActions = new Dictionary<string, int>
            {
                { "View", random.Next(2000, 5000) },
                { "Update", random.Next(500, 1500) },
                { "Create", random.Next(300, 1000) },
                { "Delete", random.Next(50, 300) },
                { "Login", random.Next(200, 800) }
            }
        };
    }

    public async Task<List<ChartDataVM>> GetAuditActivityChartAsync(int days)
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
                Value = random.Next(50, 500), // Actions per day
                Date = date
            });
        }

        return await Task.FromResult(data);
    }

    public async Task<List<UserAuditSummaryVM>> GetTopActiveUsersAsync(DateTime startDate, DateTime endDate, int limit = 10)
    {
        var users = new List<UserAuditSummaryVM>();
        var random = new Random();
        var userNames = new[] { "admin", "moderator", "editor", "user1", "user2", "user3", "user4", "user5" };

        for (int i = 0; i < Math.Min(limit, userNames.Length); i++)
        {
            users.Add(new UserAuditSummaryVM
            {
                UserId = Guid.NewGuid().ToString(),
                UserName = $"{userNames[i]}@example.com",
                TotalActions = random.Next(100, 1000),
                CreateActions = random.Next(10, 100),
                UpdateActions = random.Next(20, 200),
                DeleteActions = random.Next(5, 50),
                ViewActions = random.Next(50, 500),
                LastActivity = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440))
            });
        }

        return await Task.FromResult(users.OrderByDescending(u => u.TotalActions).ToList());
    }

    public async Task<bool> ExportAuditLogsAsync(DateTime startDate, DateTime endDate, string format = "csv")
    {
        // In real implementation, generate and save export file
        await Task.Delay(1000);
        return true;
    }

    public async Task<bool> PurgeOldAuditLogsAsync(int daysToKeep)
    {
        // In real implementation, delete old audit logs from database
        await Task.Delay(500);
        return true;
    }

    public async Task<List<AuditLogVM>> GetUserAuditHistoryAsync(string userId, int limit = 100)
    {
        return await GetAuditLogsAsync(userId: userId, pageSize: limit);
    }

    public async Task<List<AuditLogVM>> GetEntityAuditHistoryAsync(string entityType, string entityId, int limit = 50)
    {
        var logs = new List<AuditLogVM>();
        var random = new Random();
        var actions = new[] { "Create", "Update", "Delete", "View" };
        var users = new[] { "admin", "user1", "user2", "moderator" };

        for (int i = 0; i < Math.Min(limit, 20); i++)
        {
            logs.Add(new AuditLogVM
            {
                Id = Guid.NewGuid(),
                UserId = users[random.Next(users.Length)],
                UserName = $"{users[random.Next(users.Length)]}@example.com",
                Action = actions[random.Next(actions.Length)],
                EntityType = entityType,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 10080)), // Last week
                IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                Success = true
            });
        }

        return await Task.FromResult(logs.OrderByDescending(l => l.Timestamp).ToList());
    }
}