using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Stories.DTOs;
using CommunityCar.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CommunityCar.Web.Controllers.Community.Stories;

[Route("stories")]
public class StoriesController : Controller
{
    private readonly ILogger<StoriesController> _logger;
    private readonly IStoriesService _storiesService;

    public StoriesController(ILogger<StoriesController> logger, IStoriesService storiesService)
    {
        _logger = logger;
        _storiesService = storiesService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 20)
    {
        var userId = GetCurrentUserId();
        
        var request = new StoriesSearchRequest
        {
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize,
            IsActive = true,
            SortBy = StoriesSortBy.Default
        };

        var response = await _storiesService.SearchStoriesAsync(request);
        
        ViewBag.CurrentUserId = userId;
        ViewBag.SearchTerm = searchTerm;
        
        return View("~/Views/Community/Stories/Index.cshtml", response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var story = await _storiesService.GetByIdAsync(id);
        if (story == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();
        ViewBag.CurrentUserId = userId;

        // Increment view count
        if (userId.HasValue)
        {
            await _storiesService.IncrementViewCountAsync(id);
        }

        return View("~/Views/Community/Stories/Details.cshtml", story);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.CurrentUserId = userId;
        return View("~/Views/Community/Stories/Create.cshtml");
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateStoryRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.CurrentUserId = userId;
            return View("~/Views/Community/Stories/Create.cshtml", request);
        }

        try
        {
            request.AuthorId = userId.Value;
            var story = await _storiesService.CreateAsync(request);
            
            TempData["SuccessMessage"] = "Story created successfully!";
            return RedirectToAction("Details", new { id = story.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating story");
            ModelState.AddModelError("", "An error occurred while creating the story.");
            ViewBag.CurrentUserId = userId;
            return View("~/Views/Community/Stories/Create.cshtml", request);
        }
    }

    [HttpGet("author/{authorId:guid}")]
    public async Task<IActionResult> ByAuthor(Guid authorId, int page = 1, int pageSize = 20)
    {
        var request = new StoriesSearchRequest
        {
            AuthorId = authorId,
            Page = page,
            PageSize = pageSize,
            IsActive = true,
            SortBy = StoriesSortBy.Newest
        };

        var response = await _storiesService.SearchStoriesAsync(request);
        
        ViewBag.CurrentUserId = GetCurrentUserId();
        ViewBag.AuthorId = authorId;
        
        return View("~/Views/Community/Stories/ByAuthor.cshtml", response);
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

    [HttpGet("GetStoryFeed")]
    public async Task<IActionResult> GetStoryFeed(int count = 20)
    {
        // For now using GetActiveStoriesAsync, but you might want specific feed logic
        var stories = await _storiesService.GetActiveStoriesAsync();
        // Return wrapped response as JS expects
        return Ok(new { success = true, data = stories.Take(count) });
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var stories = await _storiesService.GetActiveStoriesAsync();
        return Ok(new { success = true, data = stories });
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> Like(Guid id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();
        
        var success = await _storiesService.LikeAsync(id, userId.Value);
        return Ok(new { success });
    }

    [HttpDelete("{id}/like")]
    public async Task<IActionResult> Unlike(Guid id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();
        
        var success = await _storiesService.UnlikeAsync(id, userId.Value);
        return Ok(new { success });
    }

    [HttpPost("{id}/share")]
    public IActionResult Share(Guid id)
    {
        // Placeholder url generation
        return Ok(new { success = true, shareUrl = Url.Action("Details", "Stories", new { id }, Request.Scheme) });
    }

    [HttpPost("{id}/view")]
    public async Task<IActionResult> ViewStory(Guid id)
    {
        await _storiesService.IncrementViewCountAsync(id);
        return Ok(new { success = true });
    }
}