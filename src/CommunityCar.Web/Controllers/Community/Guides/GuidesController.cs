using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;

using CommunityCar.Application.Features.Guides.ViewModels;
using CommunityCar.Domain.Enums.Community;
using Microsoft.Extensions.Localization;

namespace CommunityCar.Web.Controllers.Community.Guides;

[Route("{culture}/guides")]
public class GuidesController : Controller
{
    private readonly IGuidesService _guidesService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<GuidesController> _localizer;
    private readonly ILogger<GuidesController> _logger;

    public GuidesController(
        IGuidesService guidesService,
        ICurrentUserService currentUserService,
        IStringLocalizer<GuidesController> localizer,
        ILogger<GuidesController> logger)
    {
        _guidesService = guidesService;
        _currentUserService = currentUserService;
        _localizer = localizer;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search = null,
        string? category = null,
        GuideDifficulty? difficulty = null,
        string? tag = null,
        string? sortBy = "newest",
        int page = 1,
        int pageSize = 12)
    {
        try
        {
            var currentUserId = !string.IsNullOrEmpty(_currentUserService.UserId) ? Guid.Parse(_currentUserService.UserId) : (Guid?)null;
            
            var filter = new GuideFilterVM
            {
                Search = search,
                Category = category,
                Difficulty = difficulty,
                Tag = tag,
                SortBy = sortBy ?? "newest",
                Page = page,
                PageSize = pageSize,
                IsPublished = true
            };

            var result = await _guidesService.GetGuidesAsync(filter, currentUserId);
            
            return View("~/Views/Community/Guides/Index.cshtml", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading guides");
            TempData["Error"] = _localizer["An error occurred while loading guides."].Value;
            return View("~/Views/Community/Guides/Index.cshtml", new GuideListVM());
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var currentUserId = !string.IsNullOrEmpty(_currentUserService.UserId) ? Guid.Parse(_currentUserService.UserId) : (Guid?)null;
            
            var guide = await _guidesService.GetGuideAsync(id, currentUserId);
            if (guide == null)
            {
                return NotFound();
            }

            // Increment view count
            await _guidesService.IncrementViewCountAsync(id);

            return View("~/Views/Community/Guides/Details.cshtml", guide);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading guide {GuideId}", id);
            return NotFound();
        }
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> DetailsBySlug(string slug)
    {
        try
        {
            var currentUserId = !string.IsNullOrEmpty(_currentUserService.UserId) ? Guid.Parse(_currentUserService.UserId) : (Guid?)null;
            
            var guide = await _guidesService.GetGuideBySlugAsync(slug, currentUserId);
            if (guide == null)
            {
                return NotFound();
            }

            // Increment view count
            await _guidesService.IncrementViewCountAsync(guide.Guide.Id);

            return View("~/Views/Community/Guides/Details.cshtml", guide);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading guide by slug {Slug}", slug);
            return NotFound();
        }
    }

    [HttpGet("create")]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var viewModel = await _guidesService.GetCreateViewModelAsync();
        return View("~/Views/Community/Guides/Create.cshtml", viewModel);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(GuideCreateVM model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Community/Guides/Create.cshtml", model);
        }

        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            
            var dto = new CreateGuideRequest
            {
                Title = model.Title,
                Content = model.Content,
                Summary = model.Summary,
                Category = model.Category,
                Difficulty = model.Difficulty,
                EstimatedMinutes = model.EstimatedMinutes,
                ThumbnailUrl = model.ThumbnailUrl,
                CoverImageUrl = model.CoverImageUrl,
                Tags = model.TagsInput?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>(),
                Prerequisites = model.PrerequisitesInput?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList() ?? new List<string>(),
                RequiredTools = model.RequiredToolsInput?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>()
            };

            var result = await _guidesService.CreateGuideAsync(dto, currentUserId);

            if (result.Success)
            {
                TempData["Success"] = _localizer["GuideCreatedSuccessfully"].Value;
                return RedirectToAction(nameof(Details), new { id = result.GuideId });
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View("~/Views/Community/Guides/Create.cshtml", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating guide");
            ModelState.AddModelError("", _localizer["An error occurred while creating the guide."]);
            return View("~/Views/Community/Guides/Create.cshtml", model);
        }
    }

    [HttpPost("{id:guid}/bookmark")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Bookmark(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var result = await _guidesService.BookmarkGuideAsync(id, currentUserId);

            if (result.Success)
            {
                TempData["Success"] = _localizer["GuideBookmarkedSuccessfully"].Value;
            }
            else
            {
                TempData["Error"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bookmarking guide {GuideId}", id);
            TempData["Error"] = _localizer["An error occurred while bookmarking the guide."].Value;
        }

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("{id:guid}/unbookmark")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Unbookmark(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var result = await _guidesService.UnbookmarkGuideAsync(id, currentUserId);

            if (result.Success)
            {
                TempData["Success"] = _localizer["Bookmark removed successfully"].Value;
            }
            else
            {
                TempData["Error"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unbookmarking guide {GuideId}", id);
            TempData["Error"] = _localizer["An error occurred while removing the bookmark."].Value;
        }

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("{id:guid}/rate")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Rate(Guid id, double rating)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var result = await _guidesService.RateGuideAsync(id, currentUserId, rating);

            if (result.Success)
            {
                TempData["Success"] = _localizer["GuideRatedSuccessfully"].Value;
            }
            else
            {
                TempData["Error"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rating guide {GuideId}", id);
            TempData["Error"] = _localizer["An error occurred while rating the guide."].Value;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("{id:guid}/publish")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var result = await _guidesService.PublishGuideAsync(id, currentUserId);

            if (result.Success)
            {
                TempData["Success"] = _localizer["GuidePublishedSuccessfully"].Value;
            }
            else
            {
                TempData["Error"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing guide {GuideId}", id);
            TempData["Error"] = _localizer["An error occurred while publishing the guide."].Value;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("{id:guid}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var guide = await _guidesService.GetGuideAsync(id, currentUserId);
            
            if (guide == null)
            {
                return NotFound();
            }

            if (!guide.CanEdit)
            {
                TempData["Error"] = "You don't have permission to edit this guide.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var viewModel = new GuideCreateVM
            {
                Title = guide.Guide.Title,
                Content = guide.Guide.Content,
                Summary = guide.Guide.Summary,
                Category = guide.Guide.Category,
                Difficulty = guide.Guide.Difficulty,
                EstimatedMinutes = guide.Guide.EstimatedMinutes,
                ThumbnailUrl = guide.Guide.ThumbnailUrl,
                CoverImageUrl = guide.Guide.CoverImageUrl,
                TagsInput = string.Join(", ", guide.Guide.Tags),
                PrerequisitesInput = string.Join("\n", guide.Guide.Prerequisites),
                RequiredToolsInput = string.Join("\n", guide.Guide.RequiredTools)
            };

            ViewBag.GuideId = id;
            ViewBag.CreatedAt = guide.Guide.CreatedAt.ToString("MMM dd, yyyy");
            ViewBag.UpdatedAt = guide.Guide.UpdatedAt?.ToString("MMM dd, yyyy") ?? "N/A";
            ViewBag.Status = guide.Guide.IsPublished ? "Published" : "Draft";

            return View("~/Views/Community/Guides/Edit.cshtml", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading guide for edit {GuideId}", id);
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/update")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, GuideCreateVM model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.GuideId = id;
            return View("~/Views/Community/Guides/Edit.cshtml", model);
        }

        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            
            var dto = new UpdateGuideRequest
            {
                Id = id,
                Title = model.Title,
                Content = model.Content,
                Summary = model.Summary,
                Category = model.Category,
                Difficulty = model.Difficulty,
                EstimatedMinutes = model.EstimatedMinutes,
                ThumbnailUrl = model.ThumbnailUrl,
                CoverImageUrl = model.CoverImageUrl,
                Tags = model.TagsInput?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>(),
                Prerequisites = model.PrerequisitesInput?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList() ?? new List<string>(),
                RequiredTools = model.RequiredToolsInput?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>()
            };

            var result = await _guidesService.UpdateGuideAsync(dto, currentUserId);

            if (result.Success)
            {
                TempData["Success"] = _localizer["GuideUpdatedSuccessfully"].Value;
                return RedirectToAction(nameof(Details), new { id });
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                ViewBag.GuideId = id;
                return View("~/Views/Community/Guides/Edit.cshtml", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating guide {GuideId}", id);
            ModelState.AddModelError("", _localizer["An error occurred while updating the guide."]);
            ViewBag.GuideId = id;
            return View("~/Views/Community/Guides/Edit.cshtml", model);
        }
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var result = await _guidesService.DeleteGuideAsync(id, currentUserId);

            if (result.Success)
            {
                TempData["Success"] = _localizer["GuideDeletedSuccessfully"].Value;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting guide {GuideId}", id);
            TempData["Error"] = _localizer["An error occurred while deleting the guide."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost("{id:guid}/verify")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Verify(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var result = await _guidesService.VerifyGuideAsync(id, currentUserId);

            if (result.Success)
            {
                TempData["Success"] = "Guide verified successfully!";
            }
            else
            {
                TempData["Error"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying guide {GuideId}", id);
            TempData["Error"] = "An error occurred while verifying the guide.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("my-guides")]
    [Authorize]
    public async Task<IActionResult> MyGuides(string? filter = "all", int page = 1, int pageSize = 10)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            
            var filterDto = new GuideFilterVM
            {
                AuthorId = currentUserId,
                Page = page,
                PageSize = pageSize,
                SortBy = "newest"
            };

            // Apply filter
            switch (filter?.ToLower())
            {
                case "published":
                    filterDto.IsPublished = true;
                    break;
                case "drafts":
                    filterDto.IsPublished = false;
                    break;
                case "featured":
                    filterDto.IsFeatured = true;
                    filterDto.IsPublished = true;
                    break;
                case "all":
                default:
                    // No additional filter
                    break;
            }

            var result = await _guidesService.GetGuidesAsync(filterDto, currentUserId);
            ViewBag.Filter = filter;
            
            return View("~/Views/Community/Guides/MyGuides.cshtml", result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user guides");
            TempData["Error"] = _localizer["An error occurred while loading your guides."].Value;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost("{id:guid}/feature")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Feature(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var result = await _guidesService.FeatureGuideAsync(id, currentUserId);

            if (result.Success)
            {
                TempData["Success"] = "Guide featured successfully!";
            }
            else
            {
                TempData["Error"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error featuring guide {GuideId}", id);
            TempData["Error"] = "An error occurred while featuring the guide.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}



