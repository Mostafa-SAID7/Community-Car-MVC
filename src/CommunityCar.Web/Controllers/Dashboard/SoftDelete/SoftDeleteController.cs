using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard.SoftDelete;
using CommunityCar.Application.Features.Dashboard.SoftDelete.ViewModels;
using CommunityCar.Web.Attributes;

namespace CommunityCar.Web.Controllers.Dashboard.SoftDelete;

[Authorize]
[Route("Dashboard/[controller]")]
public class SoftDeleteController : Controller
{
    private readonly ISoftDeleteService _softDeleteService;
    private readonly ILogger<SoftDeleteController> _logger;

    public SoftDeleteController(
        ISoftDeleteService softDeleteService,
        ILogger<SoftDeleteController> logger)
    {
        _softDeleteService = softDeleteService;
        _logger = logger;
    }

    #region Views

    /// <summary>
    /// Display deleted content management page (Admin only)
    /// </summary>
    [HttpGet]
    [RequirePermission("Admin.ManageDeletedContent")]
    public async Task<IActionResult> Index(DeletedContentFilterVM filter)
    {
        try
        {
            var posts = await _softDeleteService.GetDeletedPostsAsync(filter.Page, filter.PageSize);
            var stories = await _softDeleteService.GetDeletedStoriesAsync(filter.Page, filter.PageSize);
            var groups = await _softDeleteService.GetDeletedGroupsAsync(filter.Page, filter.PageSize);

            var model = new DeletedContentSearchVM
            {
                Items = new List<DeletedContentItemVM>(),
                TotalCount = posts.TotalCount + stories.TotalCount + groups.TotalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Filter = filter
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading deleted content management page");
            TempData["Error"] = "An error occurred while loading the deleted content.";
            return RedirectToAction("Index", "Dashboard", new { area = "Dashboard" });
        }
    }

    /// <summary>
    /// Display user's own deleted content
    /// </summary>
    [HttpGet("MyDeleted")]
    public async Task<IActionResult> MyDeleted(DeletedContentFilterVM filter)
    {
        try
        {
            var posts = await _softDeleteService.GetDeletedPostsAsync(filter.Page, filter.PageSize);
            
            var model = new DeletedContentSearchVM
            {
                Items = new List<DeletedContentItemVM>(),
                TotalCount = posts.TotalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                Filter = filter
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user's deleted content");
            TempData["Error"] = "An error occurred while loading your deleted content.";
            return RedirectToAction("Index", "Home");
        }
    }

    /// <summary>
    /// Display soft delete statistics (Admin only)
    /// </summary>
    [HttpGet("Stats")]
    [RequirePermission("Admin.ViewDeletedContentStats")]
    public async Task<IActionResult> Stats()
    {
        try
        {
            var stats = await _softDeleteService.GetSoftDeleteStatsAsync();
            var recentlyDeleted = await _softDeleteService.GetRecentlyDeletedContentAsync(7);

            ViewBag.RecentlyDeleted = recentlyDeleted;
            return View(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading soft delete statistics");
            TempData["Error"] = "An error occurred while loading the statistics.";
            return RedirectToAction("Index");
        }
    }

    #endregion

    #region Post Operations

    /// <summary>
    /// Soft delete a post
    /// </summary>
    [HttpPost("Posts/{id}/SoftDelete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDeletePost(Guid id, [FromBody] SoftDeleteRequestVM request)
    {
        try
        {
            var result = await _softDeleteService.SoftDeletePostAsync(id);
            
            if (result)
            {
                return Json(new SoftDeleteResponseVM 
                { 
                    Success = true, 
                    Message = "Post deleted successfully",
                    Id = id,
                    DeletedAt = DateTime.UtcNow
                });
            }

            return Json(new SoftDeleteResponseVM 
            { 
                Success = false, 
                Message = "Failed to delete post" 
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new SoftDeleteResponseVM 
            { 
                Success = false, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting post {PostId}", id);
            return Json(new SoftDeleteResponseVM 
            { 
                Success = false, 
                Message = "An error occurred while deleting the post" 
            });
        }
    }

    /// <summary>
    /// Restore a post
    /// </summary>
    [HttpPost("Posts/{id}/Restore")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestorePost(Guid id, [FromBody] RestoreRequestVM request)
    {
        try
        {
            var result = await _softDeleteService.RestorePostAsync(id);
            
            if (result)
            {
                return Json(new RestoreResponseVM 
                { 
                    Success = true, 
                    Message = "Post restored successfully",
                    Id = id,
                    RestoredAt = DateTime.UtcNow
                });
            }

            return Json(new RestoreResponseVM 
            { 
                Success = false, 
                Message = "Failed to restore post" 
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new RestoreResponseVM 
            { 
                Success = false, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring post {PostId}", id);
            return Json(new RestoreResponseVM 
            { 
                Success = false, 
                Message = "An error occurred while restoring the post" 
            });
        }
    }

    /// <summary>
    /// Permanently delete a post (Admin only)
    /// </summary>
    [HttpPost("Posts/{id}/PermanentDelete")]
    [RequirePermission("Admin.PermanentDeleteContent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PermanentDeletePost(Guid id, [FromBody] PermanentDeleteRequestVM request)
    {
        try
        {
            if (!request.ConfirmPermanentDelete)
            {
                return Json(new PermanentDeleteResponseVM 
                { 
                    Success = false, 
                    Message = "Permanent deletion must be confirmed" 
                });
            }

            var result = await _softDeleteService.PermanentDeletePostAsync(id);
            
            if (result)
            {
                return Json(new PermanentDeleteResponseVM 
                { 
                    Success = true, 
                    Message = "Post permanently deleted",
                    Id = id,
                    PermanentlyDeletedAt = DateTime.UtcNow
                });
            }

            return Json(new PermanentDeleteResponseVM 
            { 
                Success = false, 
                Message = "Failed to permanently delete post" 
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new PermanentDeleteResponseVM 
            { 
                Success = false, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error permanently deleting post {PostId}", id);
            return Json(new PermanentDeleteResponseVM 
            { 
                Success = false, 
                Message = "An error occurred while permanently deleting the post" 
            });
        }
    }

    /// <summary>
    /// Bulk soft delete posts
    /// </summary>
    [HttpPost("Posts/BulkSoftDelete")]
    [RequirePermission("Admin.BulkDeleteContent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkSoftDeletePosts([FromBody] BulkSoftDeleteRequestVM request)
    {
        try
        {
            var result = await _softDeleteService.BulkSoftDeletePostsAsync(request.Ids);
            
            return Json(new BulkSoftDeleteResponseVM 
            { 
                Success = result > 0, 
                Message = $"{result} posts deleted successfully",
                TotalRequested = request.Ids.Count(),
                TotalProcessed = request.Ids.Count(),
                TotalSuccessful = result,
                TotalFailed = request.Ids.Count() - result,
                SuccessfulIds = request.Ids.Take(result)
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new BulkSoftDeleteResponseVM 
            { 
                Success = false, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk soft deleting posts");
            return Json(new BulkSoftDeleteResponseVM 
            { 
                Success = false, 
                Message = "An error occurred while deleting the posts" 
            });
        }
    }

    /// <summary>
    /// Bulk restore posts
    /// </summary>
    [HttpPost("Posts/BulkRestore")]
    [RequirePermission("Admin.BulkRestoreContent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkRestorePosts([FromBody] BulkRestoreRequestVM request)
    {
        try
        {
            var result = await _softDeleteService.BulkRestorePostsAsync(request.Ids);
            
            return Json(new BulkSoftDeleteResponseVM 
            { 
                Success = result > 0, 
                Message = $"{result} posts restored successfully",
                TotalRequested = request.Ids.Count(),
                TotalProcessed = request.Ids.Count(),
                TotalSuccessful = result,
                TotalFailed = request.Ids.Count() - result,
                SuccessfulIds = request.Ids.Take(result)
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new BulkSoftDeleteResponseVM 
            { 
                Success = false, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk restoring posts");
            return Json(new BulkSoftDeleteResponseVM 
            { 
                Success = false, 
                Message = "An error occurred while restoring the posts" 
            });
        }
    }

    #endregion

    #region Admin Operations

    /// <summary>
    /// Cleanup old deleted content (Admin only)
    /// </summary>
    [HttpPost("Cleanup")]
    [RequirePermission("Admin.CleanupDeletedContent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CleanupOldContent([FromBody] CleanupConfigurationVM config)
    {
        try
        {
            var result = await _softDeleteService.CleanupOldDeletedContentAsync(config.DaysToKeepDeleted);
            
            var report = new CleanupReportVM
            {
                CleanupDate = DateTime.UtcNow,
                PerformedBy = User.Identity?.Name ?? "Unknown",
                TotalItemsCleaned = result,
                Success = true,
                Duration = TimeSpan.FromSeconds(1) // Placeholder
            };

            return Json(report);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new CleanupReportVM 
            { 
                Success = false, 
                ErrorMessage = ex.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old deleted content");
            return Json(new CleanupReportVM 
            { 
                Success = false, 
                ErrorMessage = "An error occurred during cleanup" 
            });
        }
    }

    /// <summary>
    /// Get user content deletion summary (Admin only)
    /// </summary>
    [HttpGet("Users/{userId}/Summary")]
    [RequirePermission("Admin.ViewUserContentSummary")]
    public async Task<IActionResult> GetUserContentSummary(Guid userId)
    {
        try
        {
            // TODO: Implement when user content summary is available
            var summary = new UserContentDeletionSummaryVM
            {
                UserId = userId,
                UserName = "Unknown", // TODO: Load from user service
                UserEmail = "unknown@example.com" // TODO: Load from user service
            };

            return Json(summary);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user content summary for user {UserId}", userId);
            return Json(new { success = false, message = "An error occurred while loading user summary" });
        }
    }

    /// <summary>
    /// Delete all user content (Admin only)
    /// </summary>
    [HttpPost("Users/{userId}/DeleteAll")]
    [RequirePermission("Admin.DeleteAllUserContent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAllUserContent(Guid userId, [FromBody] SoftDeleteRequestVM request)
    {
        try
        {
            var result = await _softDeleteService.SoftDeleteAllUserContentAsync(userId);
            
            return Json(new BulkSoftDeleteResponseVM 
            { 
                Success = result > 0, 
                Message = $"Deleted {result} items for user",
                TotalSuccessful = result
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new BulkSoftDeleteResponseVM 
            { 
                Success = false, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all content for user {UserId}", userId);
            return Json(new BulkSoftDeleteResponseVM 
            { 
                Success = false, 
                Message = "An error occurred while deleting user content" 
            });
        }
    }

    #endregion

    #region API Endpoints

    /// <summary>
    /// Get deleted content via API
    /// </summary>
    [HttpGet("Api/DeletedContent")]
    public async Task<IActionResult> GetDeletedContentApi([FromQuery] DeletedContentFilterVM filter)
    {
        try
        {
            var posts = await _softDeleteService.GetDeletedPostsAsync(filter.Page, filter.PageSize);
            
            return Json(new 
            { 
                success = true, 
                data = posts,
                filter = filter
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting deleted content via API");
            return Json(new { success = false, message = "An error occurred while loading deleted content" });
        }
    }

    /// <summary>
    /// Get soft delete statistics via API
    /// </summary>
    [HttpGet("Api/Stats")]
    [RequirePermission("Admin.ViewDeletedContentStats")]
    public async Task<IActionResult> GetStatsApi()
    {
        try
        {
            var stats = await _softDeleteService.GetSoftDeleteStatsAsync();
            return Json(new { success = true, data = stats });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting soft delete statistics via API");
            return Json(new { success = false, message = "An error occurred while loading statistics" });
        }
    }

    #endregion
}