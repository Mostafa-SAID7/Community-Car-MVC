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