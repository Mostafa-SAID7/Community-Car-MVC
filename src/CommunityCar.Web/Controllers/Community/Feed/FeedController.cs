using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Common.Interfaces.Services.Account;
using CommunityCar.Application.Features.Feed.DTOs;
using CommunityCar.Application.Features.Analytics.DTOs;
using CommunityCar.Web.Models.Error;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Globalization;
using CommunityCar.Domain.Enums.Account;
using Microsoft.AspNetCore.Authorization;

namespace CommunityCar.Web.Controllers.Community.Feed;

[Authorize]
[Route("{culture}/feed")]
public class FeedController : Controller
{
    private readonly ILogger<FeedController> _logger;
    private readonly IFeedService _feedService;
    private readonly IUserAnalyticsService? _analyticsService;

    public FeedController(ILogger<FeedController> logger, IFeedService feedService, IUserAnalyticsService? analyticsService = null)
    {
        _logger = logger;
        _feedService = feedService;
        _analyticsService = analyticsService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string feedType = "personalized", string? topic = null, int page = 1, int pageSize = 0)
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

        if (!string.IsNullOrEmpty(topic))
        {
            request.Tags.Add(topic);
            ViewBag.CurrentTopic = topic;
            
            // If topic is selected, we might want to default to Trending algorithm if not specified
            if (feedType == "personalized") 
            {
               request.FeedType = FeedType.Trending;
               feedType = "trending";
            }
        }

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
        // TODO: Update to use LogUserActivityAsync from IUserAnalyticsService
        /*
        if (userId.HasValue && _analyticsService != null)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _analyticsService.TrackActivityAsync(new TrackActivityRequest
                    {
                        UserId = userId.Value,
                        ActivityType = ActivityType.View,
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
        */
        
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

        // Add topic filter if provided
        if (!string.IsNullOrEmpty(topic))
        {
            request.Tags.Add(topic);
            ViewBag.CurrentTopic = topic;
        }

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
        
        return await Index("friends", null, page, pageSize);
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

    [HttpPost("interact")]
    public async Task<IActionResult> Interact([FromBody] FeedInteractionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { success = false, message = "User not logged in" });
        
        bool success = false;
        if (request.InteractionType?.ToLower() == "share")
        {
            success = true; // Placeholder for share tracking
            
            // Track share analytics
            // TODO: Update to use LogUserActivityAsync
            /*
            if (_analyticsService != null)
            {
                await _analyticsService.TrackActivityAsync(new TrackActivityRequest
                {
                    UserId = userId.Value,
                    ActivityType = ActivityType.Share,
                    EntityType = request.ContentType,
                    EntityId = request.ContentId,
                    Description = $"Shared {request.ContentType}"
                });
            }
            */
        }
        else
        {
            success = true; // Placeholder for like tracking
        }

        return Ok(new { success });
    }

    [HttpPost("bookmark")]
    public IActionResult Bookmark([FromBody] FeedInteractionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { success = false, message = "User not logged in" });

        // var success = await _feedService.BookmarkContentAsync(userId.Value, request.ContentId, request.ContentType);
        var success = true; // Not implemented in service yet
        return Ok(new { success, isBookmarked = success }); 
    }

    [HttpPost("hide")]
    public IActionResult Hide([FromBody] FeedInteractionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { success = false, message = "User not logged in" });

        // var success = await _feedService.HideContentAsync(userId.Value, request.ContentId, request.ContentType);
        var success = true; // Not implemented in service yet
        return Ok(new { success });
    }

    [HttpPost("report")]
    public async Task<IActionResult> Report([FromBody] FeedInteractionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { success = false, message = "User not logged in" });

        var success = await _feedService.ReportContentAsync(userId.Value, request.ContentId, request.ContentType, request.Reason ?? "No reason provided");
        return Ok(new { success });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var userId = GetCurrentUserId();
        // GetFeedStatsAsync requires userId (nullable or not? checked service, it takes Guid?)
        // FeedService line 74: await GetFeedStatsAsync(request.UserId) -> FeedRequest.UserId is Guid?
        var stats = await _feedService.GetFeedStatsAsync(userId);
        return Ok(stats);
    }
    [HttpPost("mark-seen")]
    public async Task<IActionResult> MarkSeen([FromBody] FeedInteractionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(new { success = false, message = "User not logged in" });

        var success = await _feedService.MarkAsSeenAsync(userId.Value, request.ContentId, request.ContentType);
        return Ok(new { success });
    }
}

public class FeedInteractionRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? InteractionType { get; set; }
    public string? Reason { get; set; }
}


