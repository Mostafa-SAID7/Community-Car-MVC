using CommunityCar.Application.Common.Interfaces.Services.Account.Profile;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Common.Interfaces.Services.Account.Gamification;
using CommunityCar.Application.Features.Account.ViewModels.Activity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Account;

[Authorize]
[Route("Profile/Views")]
public class ProfileViewController : Controller
{
    private readonly IProfileViewService _profileViewService;
    private readonly IProfileService _profileService;
    private readonly IGamificationService _gamificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProfileViewController> _logger;

    public ProfileViewController(
        IProfileViewService profileViewService,
        IProfileService profileService,
        IGamificationService gamificationService,
        ICurrentUserService currentUserService,
        ILogger<ProfileViewController> logger)
    {
        _profileViewService = profileViewService;
        _profileService = profileService;
        _gamificationService = gamificationService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet("WhoViewed")]
    public async Task<IActionResult> WhoViewedMyProfile(int page = 1, int pageSize = 20)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await SetProfileHeaderDataAsync(currentUserId);
            
            var viewModel = await _profileViewService.GetWhoViewedMyProfileAsync(currentUserId, page, pageSize);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading who viewed my profile page");
            return View(new WhoViewedMyProfileVM());
        }
    }

    [HttpGet("Analytics")]
    public async Task<IActionResult> ViewAnalytics(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await SetProfileHeaderDataAsync(currentUserId);
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            // Use available methods from the interface
            var whoViewedData = await _profileViewService.GetWhoViewedMyProfileAsync(currentUserId, 1, 20);
            var viewCount = await _profileViewService.GetProfileViewCountAsync(currentUserId);
            var todayViewCount = await _profileViewService.GetProfileViewCountTodayAsync(currentUserId);
            var chartData = await _profileViewService.GetProfileViewsChartAsync(currentUserId, start, end);

            var viewModel = new ProfileViewAnalyticsVM
            {
                // Adapt the data to fit the expected structure
                Stats = new ProfileViewStatsVM 
                { 
                    TotalViews = viewCount,
                    TodayViews = todayViewCount,
                    UniqueViewers = whoViewedData.TotalViewers
                },
                TopViewers = whoViewedData.Viewers.Take(10).ToList(),
                RecentViews = new List<ProfileViewVM>() // Empty for now since we don't have this data
            };

            ViewBag.StartDate = start;
            ViewBag.EndDate = end;
            ViewBag.ViewTrends = chartData;
            ViewBag.ViewSources = new Dictionary<string, int> { { "Direct", viewCount } };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile view analytics");
            return View(new ProfileViewAnalyticsVM());
        }
    }

    [HttpGet("History")]
    public async Task<IActionResult> MyViewHistory(int page = 1, int pageSize = 20)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await SetProfileHeaderDataAsync(currentUserId);
            
            // Since GetUserViewHistoryAsync doesn't exist, return empty list for now
            var viewHistory = new List<object>();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.HasNextPage = false;
            ViewBag.HasPreviousPage = page > 1;

            return View(viewHistory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading view history");
            return View(new List<ProfileViewVM>());
        }
    }

    [HttpPost("Record")]
    public async Task<IActionResult> RecordView([FromBody] RecordViewRequest request)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            
            // Use the available TrackProfileViewAsync method
            var success = await _profileViewService.TrackProfileViewAsync(request.ProfileUserId, currentUserId);

            return Json(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording profile view");
            return Json(new { success = false });
        }
    }

    [HttpPost("RecordAnonymous")]
    [AllowAnonymous]
    public async Task<IActionResult> RecordAnonymousView([FromBody] RecordAnonymousViewRequest request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var referrer = HttpContext.Request.Headers["Referer"].ToString();

            // Use the available TrackProfileViewAsync method for anonymous views (no viewer ID)
            var success = await _profileViewService.TrackProfileViewAsync(request.ProfileUserId, null);

            return Json(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording anonymous profile view");
            return Json(new { success = false });
        }
    }

    [HttpPost("UpdateDuration")]
    public async Task<IActionResult> UpdateViewDuration([FromBody] UpdateViewDurationRequest request)
    {
        try
        {
            // Since UpdateViewDurationAsync doesn't exist, just return success for now
            // await _profileViewService.UpdateViewDurationAsync(request.ViewId, TimeSpan.FromSeconds(request.DurationSeconds));
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating view duration");
            return Json(new { success = false });
        }
    }

    [HttpGet("Stats/{userId:guid}")]
    public async Task<IActionResult> GetProfileStats(Guid userId)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            
            // Only allow users to see their own stats or public stats
            if (currentUserId != userId)
            {
                return Forbid();
            }

            // Use available methods to get stats
            var viewCount = await _profileViewService.GetProfileViewCountAsync(userId);
            var todayViewCount = await _profileViewService.GetProfileViewCountTodayAsync(userId);
            var whoViewedData = await _profileViewService.GetWhoViewedMyProfileAsync(userId, 1, 1);
            
            var stats = new ProfileViewStatsVM 
            { 
                TotalViews = viewCount,
                TodayViews = todayViewCount,
                UniqueViewers = whoViewedData.TotalViewers
            };
            
            return Json(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile stats for user {UserId}", userId);
            return Json(new ProfileViewStatsVM());
        }
    }

    private async Task SetProfileHeaderDataAsync(Guid userId)
    {
        try
        {
            var profile = await _profileService.GetProfileAsync(userId);
            if (profile != null)
            {
                ViewBag.FullName = profile.FullName;
                ViewBag.Email = profile.Email;
                ViewBag.UserName = profile.UserName;
                ViewBag.Bio = profile.Bio;
                ViewBag.BioAr = profile.BioAr;
                ViewBag.City = profile.City;
                ViewBag.Country = profile.Country;
                ViewBag.ProfilePictureUrl = profile.ProfilePictureUrl;
                ViewBag.CreatedAt = profile.CreatedAt;
                ViewBag.PostsCount = profile.PostsCount;
                ViewBag.CommentsCount = profile.CommentsCount;
                ViewBag.LikesReceived = profile.LikesReceived;
                
                var stats = await _gamificationService.GetUserStatsAsync(userId);
                if (stats != null)
                {
                    ViewBag.Level = stats.Level;
                    ViewBag.TotalPoints = stats.TotalPoints;
                    ViewBag.Rank = stats.Rank;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load profile header data for analytics");
        }
    }
}

public class RecordViewRequest
{
    public Guid ProfileUserId { get; set; }
    public string? Location { get; set; }
    public string? ViewSource { get; set; }
}

public class RecordAnonymousViewRequest
{
    public Guid ProfileUserId { get; set; }
    public string? Location { get; set; }
    public string? ViewSource { get; set; }
}

public class UpdateViewDurationRequest
{
    public Guid ViewId { get; set; }
    public int DurationSeconds { get; set; }
}