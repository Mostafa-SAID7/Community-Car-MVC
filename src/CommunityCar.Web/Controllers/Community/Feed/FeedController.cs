using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Features.Feed.DTOs;
using CommunityCar.Application.Features.Analytics.DTOs;
using CommunityCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Globalization;

namespace CommunityCar.Web.Controllers.Community.Feed;

[Route("feed")]
public class FeedController : Controller
{
    private readonly ILogger<FeedController> _logger;
    private readonly IFeedService _feedService;
    private readonly IUserAnalyticsService _analyticsService;

    public FeedController(ILogger<FeedController> logger, IFeedService feedService, IUserAnalyticsService analyticsService)
    {
        _logger = logger;
        _feedService = feedService;
        _analyticsService = analyticsService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string feedType = "personalized", int page = 1, int pageSize = 0)
    {
        // Debug current culture
        var currentCulture = CultureInfo.CurrentCulture.Name;
        var currentUICulture = CultureInfo.CurrentUICulture.Name;
        Console.WriteLine($"Feed Index - Current Culture: {currentCulture}, UI Culture: {currentUICulture}");
        
        var userId = GetCurrentUserId();
        
        // Set page size: 20 for first page, 10 for subsequent pages
        if (pageSize == 0)
        {
            pageSize = page == 1 ? 20 : 10;
        }
        
        var request = new FeedRequest
        {
            UserId = userId,
            FeedType = Enum.TryParse<FeedType>(feedType, true, out var type) ? type : FeedType.Personalized,
            Page = page,
            PageSize = pageSize
        };

        var feedResponse = request.FeedType switch
        {
            FeedType.Trending => await _feedService.GetTrendingFeedAsync(request),
            FeedType.Friends => await _feedService.GetFriendsFeedAsync(request),
            _ => await _feedService.GetPersonalizedFeedAsync(request)
        };

        ViewBag.FeedType = feedType;
        ViewBag.CurrentUserId = userId;
        
        // Pass feed data to sidebar
        ViewData["FeedSidebarData"] = feedResponse;
        
        // Track user activity
        if (userId.HasValue)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _analyticsService.TrackActivityAsync(new TrackActivityRequest
                    {
                        UserId = userId.Value,
                        ActivityType = "View",
                        EntityType = "Feed",
                        Description = $"Viewed {feedType} feed",
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserAgent = HttpContext.Request.Headers.UserAgent.ToString()
                    });

                    // Update user interests based on feed type
                    await _analyticsService.UpdateUserInterestsAsync(
                        userId.Value,
                        "Content",
                        "Feed",
                        "FeedType",
                        feedType,
                        0.5,
                        "FeedView"
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error tracking feed activity for user {UserId}", userId);
                }
            });
        }
        
        return View("~/Views/Community/Feed/Index.cshtml", feedResponse);
    }

    [HttpGet("trending")]
    public async Task<IActionResult> Trending(int page = 1, int pageSize = 0, string? topic = null)
    {
        var userId = GetCurrentUserId();
        
        // Set page size: 20 for first page, 10 for subsequent pages
        if (pageSize == 0)
        {
            pageSize = page == 1 ? 20 : 10;
        }
        
        var request = new FeedRequest
        {
            UserId = userId,
            FeedType = FeedType.Trending,
            Page = page,
            PageSize = pageSize
        };

        var feedResponse = await _feedService.GetTrendingFeedAsync(request);

        ViewBag.FeedType = "trending";
        ViewBag.CurrentUserId = userId;
        ViewBag.Topic = topic;
        
        // Pass feed data to sidebar
        ViewData["FeedSidebarData"] = feedResponse;
        
        return View("~/Views/Community/Feed/Trending.cshtml", feedResponse);
    }

    [HttpGet("friends")]
    public async Task<IActionResult> Friends(int page = 1, int pageSize = 0)
    {
        // Set page size: 20 for first page, 10 for subsequent pages
        if (pageSize == 0)
        {
            pageSize = page == 1 ? 20 : 10;
        }
        
        return await Index("friends", page, pageSize);
    }

    [HttpGet("privacy")]
    public IActionResult Privacy()
    {
        return View("~/Views/Community/Feed/Privacy.cshtml");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet("error")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}