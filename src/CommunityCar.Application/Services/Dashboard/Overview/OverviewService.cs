using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview;
using CommunityCar.Application.Features.Dashboard.Overview.ViewModels;
using CommunityCar.Application.Features.Shared.ViewModels;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview.Users.Statistics;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview.Users.Activity;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Overview.Users.Security;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Features.Dashboard.Overview.Users.Activity;

namespace CommunityCar.Application.Services.Dashboard.Overview;

public class OverviewService : IOverviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserOverviewStatisticsService _userStatisticsService;
    private readonly IUserOverviewActivityService _userActivityService;
    private readonly IUserOverviewSecurityService _userSecurityService;

    public OverviewService(
        IUnitOfWork unitOfWork,
        IUserOverviewStatisticsService userStatisticsService,
        IUserOverviewActivityService userActivityService,
        IUserOverviewSecurityService userSecurityService)
    {
        _unitOfWork = unitOfWork;
        _userStatisticsService = userStatisticsService;
        _userActivityService = userActivityService;
        _userSecurityService = userSecurityService;
    }

    public async Task<OverviewVM> GetOverviewAsync(OverviewVM? request = null)
    {
        var userStats = await _userStatisticsService.GetUserOverviewStatsAsync();
        var activitySummary = await _userActivityService.GetActivitySummaryAsync();
        var securityOverview = await _userSecurityService.GetSecurityOverviewAsync();

        return new OverviewVM
        {
            TotalUsers = userStats.TotalUsers,
            ActiveUsers = new List<ActiveUserVM>(),
            NewUsersToday = userStats.NewUsersToday,
            SecurityScore = securityOverview.SecurityScore
        };
    }

    public async Task<OverviewVM> GetOverviewAsync()
    {
        return await GetOverviewAsync(null);
    }

    public async Task<List<CommunityCar.Application.Features.Shared.ViewModels.StatsVM>> GetQuickStatsAsync()
    {
        return new List<CommunityCar.Application.Features.Shared.ViewModels.StatsVM>
        {
            new() { Title = "Total Users", Value = "1,234", Icon = "users", Change = "+5.2%" },
            new() { Title = "Active Today", Value = "456", Icon = "activity", Change = "+2.1%" },
            new() { Title = "New Posts", Value = "89", Icon = "file-text", Change = "+12.3%" }
        };
    }

    public async Task<List<ChartDataVM>> GetUserGrowthChartAsync(DateTime startDate, DateTime endDate)
    {
        var data = new List<ChartDataVM>();
        var current = startDate;
        var random = new Random();

        while (current <= endDate)
        {
            data.Add(new ChartDataVM
            {
                Label = current.ToString("MMM dd"),
                Value = random.Next(10, 100),
                Date = current
            });
            current = current.AddDays(1);
        }

        return data;
    }

    public async Task<List<ChartDataVM>> GetContentChartAsync(DateTime startDate, DateTime endDate)
    {
        var data = new List<ChartDataVM>();
        var current = startDate;
        var random = new Random();

        while (current <= endDate)
        {
            data.Add(new ChartDataVM
            {
                Label = current.ToString("MMM dd"),
                Value = random.Next(5, 50),
                Date = current
            });
            current = current.AddDays(1);
        }

        return data;
    }

    public async Task<List<ChartDataVM>> GetEngagementChartAsync(DateTime startDate, DateTime endDate)
    {
        var data = new List<ChartDataVM>();
        var current = startDate;
        var random = new Random();

        while (current <= endDate)
        {
            data.Add(new ChartDataVM
            {
                Label = current.ToString("MMM dd"),
                Value = random.Next(20, 200),
                Date = current
            });
            current = current.AddDays(1);
        }

        return data;
    }

    public async Task RefreshOverviewDataAsync()
    {
        await Task.CompletedTask;
    }

    public async Task<CommunityCar.Application.Features.Shared.ViewModels.StatsVM> GetStatsAsync(DateTime? startDate, DateTime? endDate)
    {
        return new CommunityCar.Application.Features.Shared.ViewModels.StatsVM
        {
            Title = "Overview Stats",
            Value = "Summary",
            Icon = "bar-chart",
            Change = "+3.5%"
        };
    }

    public async Task<List<RecentActivityVM>> GetRecentActivityAsync(int count)
    {
        return new List<RecentActivityVM>
        {
            new() { Activity = "User registered", User = "John Doe", Timestamp = DateTime.UtcNow.AddMinutes(-5) },
            new() { Activity = "Post created", User = "Jane Smith", Timestamp = DateTime.UtcNow.AddMinutes(-10) }
        };
    }

    public async Task RefreshMetricsAsync()
    {
        await Task.CompletedTask;
    }
}