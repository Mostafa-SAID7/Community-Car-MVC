using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.News.DTOs;
using CommunityCar.Application.Features.News.ViewModels;
using CommunityCar.Domain.Enums.Community;
using Microsoft.Extensions.Localization;

namespace CommunityCar.Web.Controllers.Community.News;

[Route("news")]
public class NewsController : Controller
{
    private readonly INewsService _newsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<NewsController> _localizer;
    private readonly ILogger<NewsController> _logger;

    public NewsController(
        INewsService newsService,
        ICurrentUserService currentUserService,
        IStringLocalizer<NewsController> localizer,
        ILogger<NewsController> logger)
    {
        _newsService = newsService;
        _currentUserService = currentUserService;
        _localizer = localizer;
        _logger = logger;
    }

    [HttpGet("categories")]
    public IActionResult Categories()
    {
        return View("~/Views/Community/News/Categories.cshtml");
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search = null,
        NewsCategory? category = null,
        string? carMake = null,
        string? carModel = null,
        int? carYear = null,
        string? tag = null,
        string? sortBy = "newest",
        int page = 1,
        int pageSize = 12)
    {
        try
        {
            var currentUserId = !string.IsNullOrEmpty(_currentUserService.UserId) ? Guid.Parse(_currentUserService.UserId) : (Guid?)null;
            
            var request = new NewsSearchRequest
            {
                SearchTerm = search,
                Category = category,
                CarMake = carMake,
                CarModel = carModel,
                CarYear = carYear,
                Tags = !string.IsNullOrEmpty(tag) ? new List<string> { tag } : new List<string>(),
                SortBy = sortBy ?? "newest",
                Page = page,
                PageSize = pageSize,
                IsPublished = true // Only show published news on public page
            };

            var response = await _newsService.SearchNewsAsync(request);
            
            ViewBag.SearchRequest = request;
            ViewBag.CurrentUserId = currentUserId;
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Community/News/_NewsList.cshtml", response);
            }
            
            return View("~/Views/Community/News/Index.cshtml", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading news index");
            return View("~/Views/Community/News/Index.cshtml", new NewsSearchResponse());
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var currentUserId = !string.IsNullOrEmpty(_currentUserService.UserId) ? Guid.Parse(_currentUserService.UserId) : (Guid?)null;
            var newsItem = await _newsService.GetByIdAsync(id, currentUserId);
            
            if (newsItem == null)
            {
                return NotFound();
            }

            // Increment view count
            await _newsService.IncrementViewCountAsync(id);
            
            return View("~/Views/Community/News/Details.cshtml", newsItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading news details for ID: {NewsId}", id);
            return NotFound();
        }
    }

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create()
    {
        var model = new NewsCreateVM();
        return View("~/Views/Community/News/Create.cshtml", model);
    }

    [HttpPost("create")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsCreateVM model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Community/News/Create.cshtml", model);
        }

        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var newsId = await _newsService.CreateAsync(model, currentUserId);
            
            TempData["SuccessMessage"] = _localizer["NewsCreatedSuccessfully"];
            return RedirectToAction(nameof(Details), new { id = newsId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating news");
            ModelState.AddModelError("", _localizer["ErrorCreatingNews"]);
            return View("~/Views/Community/News/Create.cshtml", model);
        }
    }

    [HttpGet("{id:guid}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var newsItem = await _newsService.GetByIdAsync(id, currentUserId);
            
            if (newsItem == null)
            {
                return NotFound();
            }

            // Check if user can edit this news item
            if (newsItem.AuthorId != currentUserId)
            {
                return Forbid();
            }

            var model = new NewsEditVM
            {
                Id = newsItem.Id,
                Headline = newsItem.Headline,
                Body = newsItem.Body,
                Summary = newsItem.Summary,
                Category = newsItem.Category,
                CarMake = newsItem.CarMake,
                CarModel = newsItem.CarModel,
                CarYear = newsItem.CarYear,
                Tags = string.Join(", ", newsItem.Tags),
                ImageUrl = newsItem.ImageUrl,
                Source = newsItem.Source,
                SourceUrl = newsItem.SourceUrl,
                MetaTitle = newsItem.MetaTitle,
                MetaDescription = newsItem.MetaDescription
            };
            
            return View("~/Views/Community/News/Edit.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading news for edit: {NewsId}", id);
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/edit")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, NewsEditVM model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Community/News/Edit.cshtml", model);
        }

        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await _newsService.UpdateAsync(id, model, currentUserId);
            
            TempData["SuccessMessage"] = _localizer["NewsUpdatedSuccessfully"];
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating news: {NewsId}", id);
            ModelState.AddModelError("", _localizer["ErrorUpdatingNews"]);
            return View("~/Views/Community/News/Edit.cshtml", model);
        }
    }

    [HttpPost("{id:guid}/delete")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await _newsService.DeleteAsync(id, currentUserId);
            
            TempData["SuccessMessage"] = _localizer["NewsDeletedSuccessfully"];
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting news: {NewsId}", id);
            TempData["ErrorMessage"] = _localizer["ErrorDeletingNews"];
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await _newsService.PublishAsync(id, currentUserId);
            
            return Json(new { success = true, message = _localizer["NewsPublishedSuccessfully"] });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing news: {NewsId}", id);
            return Json(new { success = false, message = _localizer["ErrorPublishingNews"] });
        }
    }

    [HttpPost("{id:guid}/unpublish")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await _newsService.UnpublishAsync(id, currentUserId);
            
            return Json(new { success = true, message = _localizer["NewsUnpublishedSuccessfully"] });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing news: {NewsId}", id);
            return Json(new { success = false, message = _localizer["ErrorUnpublishingNews"] });
        }
    }

    [HttpPost("{id:guid}/like")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Like(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await _newsService.LikeAsync(id, currentUserId);
            
            var likeCount = await _newsService.GetLikeCountAsync(id);
            return Json(new { success = true, likeCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error liking news: {NewsId}", id);
            return Json(new { success = false, message = _localizer["ErrorLikingNews"] });
        }
    }

    [HttpPost("{id:guid}/unlike")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlike(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await _newsService.UnlikeAsync(id, currentUserId);
            
            var likeCount = await _newsService.GetLikeCountAsync(id);
            return Json(new { success = true, likeCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unliking news: {NewsId}", id);
            return Json(new { success = false, message = _localizer["ErrorUnlikingNews"] });
        }
    }

    [HttpGet("my-news")]
    [Authorize]
    public async Task<IActionResult> MyNews(int page = 1, int pageSize = 12)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            
            var request = new NewsSearchRequest
            {
                AuthorId = currentUserId,
                Page = page,
                PageSize = pageSize,
                SortBy = "newest"
            };

            var response = await _newsService.SearchNewsAsync(request);
            
            return View("~/Views/Community/News/MyNews.cshtml", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user's news");
            return View("~/Views/Community/News/MyNews.cshtml", new NewsSearchResponse());
        }
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> Category(NewsCategory category, int page = 1, int pageSize = 12)
    {
        try
        {
            var request = new NewsSearchRequest
            {
                Category = category,
                Page = page,
                PageSize = pageSize,
                IsPublished = true,
                SortBy = "newest"
            };

            var response = await _newsService.SearchNewsAsync(request);
            
            ViewBag.Category = category;
            return View("~/Views/Community/News/Category.cshtml", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading news by category: {Category}", category);
            return View("~/Views/Community/News/Category.cshtml", new NewsSearchResponse());
        }
    }

    [HttpGet("{id:guid}/related")]
    public async Task<IActionResult> Related(Guid id)
    {
        try
        {
            var currentUserId = !string.IsNullOrEmpty(_currentUserService.UserId) ? Guid.Parse(_currentUserService.UserId) : (Guid?)null;
            var newsItem = await _newsService.GetByIdAsync(id, currentUserId);
            
            if (newsItem == null)
            {
                return NotFound();
            }

            // Get related news based on category, tags, and car make/model
            var request = new NewsSearchRequest
            {
                Category = newsItem.Category,
                CarMake = newsItem.CarMake,
                CarModel = newsItem.CarModel,
                Tags = newsItem.Tags.Take(3).ToList(), // Use first 3 tags
                IsPublished = true,
                PageSize = 6,
                SortBy = "newest"
            };

            var response = await _newsService.SearchNewsAsync(request);
            
            // Remove the current article from related news
            var relatedNews = response.Items.Where(n => n.Id != id).Take(3).ToList();
            
            return PartialView("~/Views/Community/News/_RelatedNews.cshtml", relatedNews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading related news for ID: {NewsId}", id);
            return PartialView("~/Views/Community/News/_RelatedNews.cshtml", new List<NewsItemVM>());
        }
    }

    [HttpPost("{id:guid}/bookmark")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Bookmark(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await _newsService.BookmarkAsync(id, currentUserId);
            
            return Json(new { success = true, message = _localizer["NewsBookmarkedSuccessfully"] });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bookmarking news: {NewsId}", id);
            return Json(new { success = false, message = _localizer["ErrorBookmarkingNews"] });
        }
    }

    [HttpPost("{id:guid}/unbookmark")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unbookmark(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            await _newsService.UnbookmarkAsync(id, currentUserId);
            
            return Json(new { success = true, message = _localizer["NewsUnbookmarkedSuccessfully"] });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unbookmarking news: {NewsId}", id);
            return Json(new { success = false, message = _localizer["ErrorUnbookmarkingNews"] });
        }
    }
}



