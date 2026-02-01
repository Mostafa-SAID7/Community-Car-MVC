using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Community.Reviews.DTOs;
using CommunityCar.Application.Features.Community.Reviews.ViewModels;
using Microsoft.Extensions.Localization;

namespace CommunityCar.Web.Controllers.Community.Reviews;

[Route("{culture}/reviews")]
public class ReviewsController : Controller
{
    private readonly IReviewsService _reviewsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<ReviewsController> _localizer;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(
        IReviewsService reviewsService,
        ICurrentUserService currentUserService,
        IStringLocalizer<ReviewsController> localizer,
        ILogger<ReviewsController> logger)
    {
        _reviewsService = reviewsService;
        _currentUserService = currentUserService;
        _localizer = localizer;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search = null,
        int? rating = null,
        string? carMake = null,
        string? carModel = null,
        int? carYear = null,
        bool? isVerifiedPurchase = null,
        bool? isRecommended = null,
        ReviewsSortBy sortBy = ReviewsSortBy.Default,
        int page = 1,
        int pageSize = 12)
    {
        try
        {
            var currentUserId = _currentUserService.UserId != null ? Guid.Parse(_currentUserService.UserId) : (Guid?)null;
            
            var request = new ReviewsSearchRequest
            {
                SearchTerm = search,
                Rating = rating,
                CarMake = carMake,
                CarModel = carModel,
                CarYear = carYear,
                IsVerifiedPurchase = isVerifiedPurchase,
                IsRecommended = isRecommended,
                IsApproved = true, // Only show approved reviews on public page
                SortBy = sortBy,
                Page = page,
                PageSize = pageSize
            };

            var response = await _reviewsService.SearchReviewsAsync(request);
            
            ViewBag.SearchRequest = request;
            ViewBag.CurrentUserId = currentUserId;
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Community/Reviews/_ReviewsList.cshtml", response);
            }
            
            return View("~/Views/Community/Reviews/Index.cshtml", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reviews index");
            return View("~/Views/Community/Reviews/Index.cshtml", new ReviewsSearchResponse());
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var review = await _reviewsService.GetByIdAsync(id);
            
            if (review == null)
            {
                return NotFound();
            }

            // Increment view count
            await _reviewsService.IncrementViewCountAsync(id);
            
            return View("~/Views/Community/Reviews/Details.cshtml", review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading review details for ID: {ReviewId}", id);
            return NotFound();
        }
    }

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create(Guid? targetId = null, string? targetType = null)
    {
        var model = new ReviewCreateVM();
        
        if (targetId.HasValue && !string.IsNullOrEmpty(targetType))
        {
            model.TargetId = targetId.Value;
            model.TargetType = targetType;
        }
        
        return View("~/Views/Community/Reviews/Create.cshtml", model);
    }

    [HttpPost("create")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewCreateVM model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Community/Reviews/Create.cshtml", model);
        }

        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            
            var request = new CreateReviewRequest
            {
                TargetId = model.TargetId,
                TargetType = model.TargetType,
                Rating = model.Rating,
                Title = model.Title,
                Comment = model.Comment,
                TitleAr = model.TitleAr,
                CommentAr = model.CommentAr,
                ReviewerId = currentUserId,
                IsVerifiedPurchase = model.IsVerifiedPurchase,
                IsRecommended = model.IsRecommended,
                PurchaseDate = model.PurchaseDate,
                PurchasePrice = model.PurchasePrice,
                CarMake = model.CarMake,
                CarModel = model.CarModel,
                CarYear = model.CarYear,
                Mileage = model.Mileage,
                OwnershipDuration = model.OwnershipDuration,
                QualityRating = model.QualityRating,
                ValueRating = model.ValueRating,
                ReliabilityRating = model.ReliabilityRating,
                PerformanceRating = model.PerformanceRating,
                ComfortRating = model.ComfortRating,
                ImageUrls = model.GetImageUrlsList(),
                Pros = model.GetProsList(),
                Cons = model.GetConsList()
            };
            
            var review = await _reviewsService.CreateAsync(request);
            
            TempData["SuccessMessage"] = _localizer["ReviewCreatedSuccessfully"];
            return RedirectToAction(nameof(Details), new { id = review.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating review");
            ModelState.AddModelError("", _localizer["ErrorCreatingReview"]);
            return View("~/Views/Community/Reviews/Create.cshtml", model);
        }
    }

    [HttpGet("{id:guid}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var review = await _reviewsService.GetByIdAsync(id);
            
            if (review == null)
            {
                return NotFound();
            }

            // Check if user can edit this review
            if (review.ReviewerId != currentUserId)
            {
                return Forbid();
            }

            var model = new ReviewEditVM
            {
                Id = review.Id,
                Rating = review.Rating,
                Title = review.Title,
                Comment = review.Comment,
                IsRecommended = review.IsRecommended,
                CarMake = review.CarMake,
                CarModel = review.CarModel,
                CarYear = review.CarYear,
                Mileage = review.Mileage,
                OwnershipDuration = review.OwnershipDuration,
                QualityRating = review.QualityRating,
                ValueRating = review.ValueRating,
                ReliabilityRating = review.ReliabilityRating,
                PerformanceRating = review.PerformanceRating,
                ComfortRating = review.ComfortRating,
                ImageUrls = string.Join(", ", review.ImageUrls),
                Pros = string.Join(", ", review.Pros),
                Cons = string.Join(", ", review.Cons)
            };
            
            return View("~/Views/Community/Reviews/Edit.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading review for edit: {ReviewId}", id);
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/edit")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ReviewEditVM model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Community/Reviews/Edit.cshtml", model);
        }

        try
        {
            var request = new UpdateReviewRequest
            {
                Rating = model.Rating,
                Title = model.Title,
                Comment = model.Comment,
                TitleAr = model.TitleAr,
                CommentAr = model.CommentAr,
                IsRecommended = model.IsRecommended,
                CarMake = model.CarMake,
                CarModel = model.CarModel,
                CarYear = model.CarYear,
                Mileage = model.Mileage,
                OwnershipDuration = model.OwnershipDuration,
                QualityRating = model.QualityRating,
                ValueRating = model.ValueRating,
                ReliabilityRating = model.ReliabilityRating,
                PerformanceRating = model.PerformanceRating,
                ComfortRating = model.ComfortRating,
                ImageUrls = model.GetImageUrlsList(),
                Pros = model.GetProsList(),
                Cons = model.GetConsList()
            };
            
            await _reviewsService.UpdateAsync(id, request);
            
            TempData["SuccessMessage"] = _localizer["ReviewUpdatedSuccessfully"];
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review: {ReviewId}", id);
            ModelState.AddModelError("", _localizer["ErrorUpdatingReview"]);
            return View("~/Views/Community/Reviews/Edit.cshtml", model);
        }
    }

    [HttpPost("{id:guid}/delete")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _reviewsService.DeleteAsync(id);
            
            if (success)
            {
                TempData["SuccessMessage"] = _localizer["ReviewDeletedSuccessfully"];
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["ErrorDeletingReview"];
            }
            
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review: {ReviewId}", id);
            TempData["ErrorMessage"] = _localizer["ErrorDeletingReview"];
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost("{id:guid}/helpful")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkHelpful(Guid id, bool isHelpful = true)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var success = await _reviewsService.MarkHelpfulAsync(id, currentUserId, isHelpful);
            
            if (success)
            {
                return Json(new { success = true, message = _localizer["HelpfulnessMarked"] });
            }
            
            return Json(new { success = false, message = _localizer["ErrorMarkingHelpfulness"] });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking review helpfulness: {ReviewId}", id);
            return Json(new { success = false, message = _localizer["ErrorMarkingHelpfulness"] });
        }
    }

    [HttpPost("{id:guid}/flag")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Flag(Guid id)
    {
        try
        {
            var success = await _reviewsService.FlagAsync(id);
            
            if (success)
            {
                return Json(new { success = true, message = _localizer["ReviewFlaggedSuccessfully"] });
            }
            
            return Json(new { success = false, message = _localizer["ErrorFlaggingReview"] });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error flagging review: {ReviewId}", id);
            return Json(new { success = false, message = _localizer["ErrorFlaggingReview"] });
        }
    }

    [HttpGet("my-reviews")]
    [Authorize]
    public async Task<IActionResult> MyReviews(int page = 1, int pageSize = 12)
    {
        try
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            
            var request = new ReviewsSearchRequest
            {
                ReviewerId = currentUserId,
                Page = page,
                PageSize = pageSize,
                SortBy = ReviewsSortBy.Newest
            };

            var response = await _reviewsService.SearchReviewsAsync(request);
            
            return View("~/Views/Community/Reviews/MyReviews.cshtml", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user's reviews");
            return View("~/Views/Community/Reviews/MyReviews.cshtml", new ReviewsSearchResponse());
        }
    }

    [HttpGet("target/{targetId:guid}")]
    public async Task<IActionResult> ByTarget(Guid targetId, string targetType = "Vehicle")
    {
        try
        {
            var request = new ReviewsSearchRequest
            {
                TargetId = targetId,
                TargetType = targetType,
                IsApproved = true,
                SortBy = ReviewsSortBy.Newest,
                PageSize = 20
            };

            var response = await _reviewsService.SearchReviewsAsync(request);
            
            ViewBag.TargetId = targetId;
            ViewBag.TargetType = targetType;
            
            return View("~/Views/Community/Reviews/ByTarget.cshtml", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reviews for target: {TargetId}", targetId);
            return View("~/Views/Community/Reviews/ByTarget.cshtml", new ReviewsSearchResponse());
        }
    }
}



