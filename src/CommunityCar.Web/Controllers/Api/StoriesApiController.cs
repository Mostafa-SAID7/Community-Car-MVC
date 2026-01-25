using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Stories.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityCar.Web.Controllers.Api;

[ApiController]
[Route("api/stories")]
public class StoriesApiController : ControllerBase
{
    private readonly ILogger<StoriesApiController> _logger;
    private readonly IStoriesService _storiesService;

    public StoriesApiController(ILogger<StoriesApiController> logger, IStoriesService storiesService)
    {
        _logger = logger;
        _storiesService = storiesService;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveStories()
    {
        try
        {
            var stories = await _storiesService.GetActiveStoriesAsync();
            return Ok(new { success = true, data = stories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active stories");
            return StatusCode(500, new { success = false, message = "Error loading stories" });
        }
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetStoriesForFeed(int count = 10)
    {
        try
        {
            var request = new StoriesSearchRequest
            {
                Page = 1,
                PageSize = count,
                IsActive = true,
                SortBy = StoriesSortBy.Default
            };

            var response = await _storiesService.SearchStoriesAsync(request);
            return Ok(new { success = true, data = response.Stories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stories for feed");
            return StatusCode(500, new { success = false, message = "Error loading stories" });
        }
    }

    [HttpPost("{id:guid}/view")]
    public async Task<IActionResult> IncrementView(Guid id)
    {
        try
        {
            var success = await _storiesService.IncrementViewCountAsync(id);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing story view count for {StoryId}", id);
            return StatusCode(500, new { success = false, message = "Error updating view count" });
        }
    }

    [HttpPost("{id:guid}/like")]
    public async Task<IActionResult> LikeStory(Guid id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new { success = false, message = "User not authenticated" });
        }

        try
        {
            var success = await _storiesService.LikeAsync(id, userId.Value);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error liking story {StoryId} by user {UserId}", id, userId);
            return StatusCode(500, new { success = false, message = "Error liking story" });
        }
    }

    [HttpDelete("{id:guid}/like")]
    public async Task<IActionResult> UnlikeStory(Guid id)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new { success = false, message = "User not authenticated" });
        }

        try
        {
            var success = await _storiesService.UnlikeAsync(id, userId.Value);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unliking story {StoryId} by user {UserId}", id, userId);
            return StatusCode(500, new { success = false, message = "Error unliking story" });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await _storiesService.GetStoriesStatsAsync();
            return Ok(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stories stats");
            return StatusCode(500, new { success = false, message = "Error loading stats" });
        }
    }

    [HttpGet("tags/popular")]
    public async Task<IActionResult> GetPopularTags(int count = 20)
    {
        try
        {
            var tags = await _storiesService.GetPopularTagsAsync(count);
            return Ok(new { success = true, data = tags });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular tags");
            return StatusCode(500, new { success = false, message = "Error loading tags" });
        }
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchStories([FromBody] StoriesSearchRequest request)
    {
        try
        {
            var response = await _storiesService.SearchStoriesAsync(request);
            return Ok(new { success = true, data = response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching stories");
            return StatusCode(500, new { success = false, message = "Error searching stories" });
        }
    }

    [HttpPost("{id:guid}/share")]
    public async Task<IActionResult> ShareStory(Guid id)
    {
        try
        {
            var story = await _storiesService.GetByIdAsync(id);
            if (story == null)
            {
                return NotFound(new { success = false, message = "Story not found" });
            }

            // In a real implementation, you might track shares or send notifications
            return Ok(new { success = true, shareUrl = $"{Request.Scheme}://{Request.Host}/stories/{id}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sharing story {StoryId}", id);
            return StatusCode(500, new { success = false, message = "Error sharing story" });
        }
    }

    [HttpPost("{id:guid}/report")]
    public async Task<IActionResult> ReportStory(Guid id, [FromBody] ReportRequest request)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new { success = false, message = "User not authenticated" });
        }

        try
        {
            // In a real implementation, you would save the report to a reports table
            _logger.LogInformation("Story {StoryId} reported by user {UserId} for reason: {Reason}", 
                id, userId, request.Reason);
            
            return Ok(new { success = true, message = "Story reported successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting story {StoryId} by user {UserId}", id, userId);
            return StatusCode(500, new { success = false, message = "Error reporting story" });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    public class ReportRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}