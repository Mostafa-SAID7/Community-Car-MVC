using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Services.Dashboard;

public class OverviewService : IOverviewService
{
    public async Task<OverviewVM> GetOverviewAsync(OverviewVM? request = null)
    {
        var startDate = request?.StartDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = request?.EndDate ?? DateTime.UtcNow;

        var overview = new OverviewVM
        {
            Stats = new StatsVM
            {
                TotalUsers = 1250,
                TotalPosts = 4500,
                TotalComments = 12500,
                TotalQuestions = 850,
                TotalAnswers = 1200,
                TotalReviews = 450,
                TotalStories = 320,
                TotalNews = 150,
                TotalInteractions = 25000,
                ActiveUsersToday = 125,
                EngagementRate = 4.5m,
                GrowthRate = 12.5m,
                LastUpdated = DateTime.UtcNow
            },
            RecentActivity = await GetRecentActivityAsync(10),
            TopContent = new List<TopContentVM>
            {
                new() { Id = Guid.NewGuid(), Title = "Top 10 Car Care Tips", AuthorName = "John Doe", Views = 1500, Likes = 250, Comments = 45, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new() { Id = Guid.NewGuid(), Title = "Best SUV for 2024", AuthorName = "Jane Smith", Views = 1200, Likes = 180, Comments = 35, CreatedAt = DateTime.UtcNow.AddDays(-3) }
            },
            ActiveUsers = new List<ActiveUserVM>
            {
                new() { Id = Guid.NewGuid(), UserName = "user1", Email = "user1@example.com", IsOnline = true, LastActivity = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), UserName = "user2", Email = "user2@example.com", IsOnline = false, LastActivity = DateTime.UtcNow.AddMinutes(-15) }
            },
            UserGrowthChart = await GetUserGrowthChartAsync(startDate, endDate),
            ContentChart = await GetContentChartAsync(startDate, endDate),
            EngagementChart = await GetEngagementChartAsync(startDate, endDate)
        };

        return overview;
    }

    public async Task<OverviewVM> GetOverviewAsync()
    {
        return await GetOverviewAsync(null);
    }


    public async Task<List<StatsVM>> GetQuickStatsAsync()
    {
        return new List<StatsVM>
        {
            new () { Title = "Total Users", Value = "1,250", Icon = "users", Color = "primary", ChangePercentage = 12.5m, IsPositiveChange = true },
            new () { Title = "Active Now", Value = "125", Icon = "activity", Color = "success", ChangePercentage = 5.2m, IsPositiveChange = true },
            new() { Title = "New Orders", Value = "45", Icon = "shopping-cart", Color = "warning", ChangePercentage = 2.1m, IsPositiveChange = false },
            new() { Title = "Reports", Value = "12", Icon = "flag", Color = "danger", ChangePercentage = 0, IsPositiveChange = true }
        };
    }

    public async Task<StatsVM> GetStatsAsync(DateTime? startDate, DateTime? endDate)
    {
        var overview = await GetOverviewAsync(new OverviewVM 
        { 
            StartDate = startDate ?? DateTime.UtcNow.AddDays(-30), 
            EndDate = endDate ?? DateTime.UtcNow 
        });
        return overview.Stats;
    }

    public async Task<List<RecentActivityVM>> GetRecentActivityAsync(int count)
    {
        var activities = new List<RecentActivityVM>();
        var types = new[] { "UserRegistered", "PostCreated", "CommentAdded", "ReportSubmitted" };
        var random = new Random();

        for (int i = 0; i < count; i++)
        {
            activities.Add(new RecentActivityVM
            {
                Type = types[random.Next(types.Length)],
                Description = $"User {i + 1} performed an action",
                UserName = $"user{i + 1}",
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                TimeAgo = $"{i + 1}h ago",
                Icon = "info",
                Color = "primary"
            });
        }

        return await Task.FromResult(activities.OrderByDescending(a => a.Timestamp).ToList());
    }

    public async Task<List<ChartDataVM>> GetUserGrowthChartAsync(DateTime startDate, DateTime endDate)
    {
        var data = new List<ChartDataVM>();
        var random = new Random();
        var current = startDate;

        while (current <= endDate)
        {
            data.Add(new ChartDataVM
            {
                Label = current.ToString("MMM dd"),
                Value = random.Next(10, 50),
                Date = current
            });
            current = current.AddDays(7);
        }

        return await Task.FromResult(data);
    }

    public async Task<List<ChartDataVM>> GetContentChartAsync(DateTime startDate, DateTime endDate)
    {
        var data = new List<ChartDataVM>();
        var random = new Random();
        var current = startDate;

        while (current <= endDate)
        {
            data.Add(new ChartDataVM
            {
                Label = current.ToString("MMM dd"),
                Value = random.Next(50, 200),
                Date = current
            });
            current = current.AddDays(7);
        }

        return await Task.FromResult(data);
    }

    public async Task<List<ChartDataVM>> GetEngagementChartAsync(DateTime startDate, DateTime endDate)
    {
        var data = new List<ChartDataVM>();
        var random = new Random();
        var current = startDate;

        while (current <= endDate)
        {
            data.Add(new ChartDataVM
            {
                Label = current.ToString("MMM dd"),
                Value = random.Next(100, 500),
                Date = current
            });
            current = current.AddDays(7);
        }

        return await Task.FromResult(data);
    }

    public async Task RefreshOverviewDataAsync()
    {
        await Task.CompletedTask;
    }

    public async Task RefreshMetricsAsync()
    {
        await Task.CompletedTask;
    }
}


