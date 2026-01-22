using System.Diagnostics;
using System.Security.Claims;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Feed.DTOs;
using CommunityCar.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Community.Feed;

public class FeedController : Controller
{
    private readonly ILogger<FeedController> _logger;
    private readonly IFeedService _feedService;

    public FeedController(ILogger<FeedController> logger, IFeedService feedService)
    {
        _logger = logger;
        _feedService = feedService;
    }

    [HttpGet("")]
    [HttpGet("feed")]
    public async Task<IActionResult> Index(string feedType = "personalized", int page = 1, int pageSize = 20)
    {
        var userId = GetCurrentUserId();
        
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
        
        return View("~/Views/Community/Feed/Index.cshtml", feedResponse);
    }

    [HttpGet("trending")]
    public async Task<IActionResult> Trending(int page = 1, int pageSize = 20)
    {
        return await Index("trending", page, pageSize);
    }

    [HttpGet("friends")]
    public async Task<IActionResult> Friends(int page = 1, int pageSize = 20)
    {
        return await Index("friends", page, pageSize);
    }

    [HttpGet("api/stories")]
    public async Task<IActionResult> GetStories()
    {
        var userId = GetCurrentUserId();
        var stories = await _feedService.GetActiveStoriesAsync(userId);
        return Json(stories);
    }

    [HttpGet("api/trending-topics")]
    public async Task<IActionResult> GetTrendingTopics(int count = 10)
    {
        var topics = await _feedService.GetTrendingTopicsAsync(count);
        return Json(topics);
    }

    [HttpGet("api/suggested-friends")]
    public async Task<IActionResult> GetSuggestedFriends(int count = 5)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Json(new List<object>());

        var suggestions = await _feedService.GetSuggestedFriendsAsync(userId.Value, count);
        return Json(suggestions);
    }

    [HttpPost("api/mark-seen")]
    public async Task<IActionResult> MarkAsSeen([FromBody] MarkSeenRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _feedService.MarkAsSeenAsync(userId.Value, request.ContentId, request.ContentType);
        return Json(new { success = result });
    }

    [HttpPost("api/interact")]
    public async Task<IActionResult> InteractWithContent([FromBody] InteractionRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var result = await _feedService.InteractWithContentAsync(userId.Value, request.ContentId, request.ContentType, request.InteractionType);
        return Json(new { success = result });
    }

    [HttpPost("api/comments")]
    public async Task<IActionResult> SubmitComment([FromBody] CommentRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        try
        {
            var result = await _feedService.AddCommentAsync(userId.Value, request.ContentId, request.ContentType, request.Comment);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting comment");
            return Json(new { success = false, error = "Failed to submit comment" });
        }
    }

    [HttpGet("api/comments/{contentId}")]
    public async Task<IActionResult> GetComments(Guid contentId, string contentType = "Post")
    {
        try
        {
            var comments = await _feedService.GetCommentsAsync(contentId, contentType);
            return Json(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading comments");
            return Json(new List<object>());
        }
    }

    [HttpPost("api/bookmark")]
    public async Task<IActionResult> BookmarkContent([FromBody] BookmarkRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        try
        {
            var result = await _feedService.BookmarkContentAsync(userId.Value, request.ContentId, request.ContentType);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bookmarking content");
            return Json(new { success = false, error = "Failed to bookmark content" });
        }
    }

    [HttpPost("api/hide")]
    public async Task<IActionResult> HideContent([FromBody] HideContentRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        try
        {
            var result = await _feedService.HideContentAsync(userId.Value, request.ContentId, request.ContentType);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hiding content");
            return Json(new { success = false, error = "Failed to hide content" });
        }
    }

    [HttpPost("api/report")]
    public async Task<IActionResult> ReportContent([FromBody] ReportContentRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        try
        {
            var result = await _feedService.ReportContentAsync(userId.Value, request.ContentId, request.ContentType, request.Reason);
            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting content");
            return Json(new { success = false, error = "Failed to report content" });
        }
    }

    [HttpGet("api/feed")]
    public async Task<IActionResult> GetFeedApi(string feedType = "personalized", int page = 1, int pageSize = 20, string contentTypes = "")
    {
        var userId = GetCurrentUserId();
        
        var request = new FeedRequest
        {
            UserId = userId,
            FeedType = Enum.TryParse<FeedType>(feedType, true, out var type) ? type : FeedType.Personalized,
            Page = page,
            PageSize = pageSize,
            ContentTypes = !string.IsNullOrEmpty(contentTypes) 
                ? contentTypes.Split(',').ToList() 
                : new List<string>()
        };

        var feedResponse = request.FeedType switch
        {
            FeedType.Trending => await _feedService.GetTrendingFeedAsync(request),
            FeedType.Friends => await _feedService.GetFriendsFeedAsync(request),
            _ => await _feedService.GetPersonalizedFeedAsync(request)
        };

        return Json(feedResponse);
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

public class MarkSeenRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

public class InteractionRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string InteractionType { get; set; } = string.Empty; // like, share, bookmark
}

public class CommentRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

public class BookmarkRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

public class HideContentRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
}

public class ReportContentRequest
{
    public Guid ContentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
