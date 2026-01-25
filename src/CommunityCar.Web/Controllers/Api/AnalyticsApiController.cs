using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Features.Analytics.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityCar.Web.Controllers.Api;

[ApiController]
[Route("api/analytics")]
public class AnalyticsApiController : ControllerBase
{
    private readonly ILogger<AnalyticsApiController> _logger;
    private readonly IUserAnalyticsService _analyticsService;

    public AnalyticsApiController(ILogger<AnalyticsApiController> logger, IUserAnalyticsService analyticsService)
    {
        _logger = logger;
        _analyticsService = analyticsService;
    }

    [HttpPost("track")]
    public async Task<IActionResult> TrackActivity([FromBody] TrackActivityRequest request)
    {
        try
        {
            // Validate user
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != request.UserId)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            // Set additional context
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = HttpContext.Request.Headers.UserAgent.ToString();

            await _analyticsService.TrackActivityAsync(request);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking activity for user {UserId}", request.UserId);
            return StatusCode(500, new { success = false, message = "Error tracking activity" });
        }
    }

    [HttpPost("track/view")]
    public async Task<IActionResult> TrackView([FromBody] TrackViewRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            await _analyticsService.TrackViewAsync(
                userId.Value,
                request.EntityType,
                request.EntityId,
                request.EntityTitle,
                request.Duration
            );

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking view for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { success = false, message = "Error tracking view" });
        }
    }

    [HttpPost("track/interaction")]
    public async Task<IActionResult> TrackInteraction([FromBody] TrackInteractionRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            await _analyticsService.TrackInteractionAsync(
                userId.Value,
                request.InteractionType,
                request.EntityType,
                request.EntityId,
                request.Metadata
            );

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking interaction for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { success = false, message = "Error tracking interaction" });
        }
    }

    [HttpGet("interests/{userId:guid}")]
    public async Task<IActionResult> GetUserInterests(Guid userId, string? category = null, int limit = 10)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var interests = await _analyticsService.GetUserInterestsAsync(userId, category, limit);
            return Ok(new { success = true, data = interests });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user interests for {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Error loading interests" });
        }
    }

    [HttpGet("interests/{userId:guid}/top")]
    public async Task<IActionResult> GetTopUserInterests(Guid userId, int limit = 5)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var interests = await _analyticsService.GetTopUserInterestsAsync(userId, limit);
            return Ok(new { success = true, data = interests });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top user interests for {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Error loading interests" });
        }
    }

    [HttpGet("activities/{userId:guid}")]
    public async Task<IActionResult> GetUserActivities(Guid userId, int days = 30, int limit = 50)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var activities = await _analyticsService.GetUserActivitiesAsync(userId, days, limit);
            return Ok(new { success = true, data = activities });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user activities for {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Error loading activities" });
        }
    }

    [HttpGet("stats/{userId:guid}")]
    public async Task<IActionResult> GetUserStats(Guid userId, int days = 30)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var activityStats = await _analyticsService.GetUserActivityStatsAsync(userId, days);
            var engagementStats = await _analyticsService.GetUserEngagementStatsAsync(userId, days);

            return Ok(new { 
                success = true, 
                data = new { 
                    activity = activityStats, 
                    engagement = engagementStats 
                } 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user stats for {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Error loading stats" });
        }
    }

    [HttpGet("recommendations/{userId:guid}/content")]
    public async Task<IActionResult> GetContentRecommendations(Guid userId, string? contentType = null, int limit = 10)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var recommendations = await _analyticsService.GetContentRecommendationsAsync(userId, contentType, limit);
            return Ok(new { success = true, data = recommendations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content recommendations for {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Error loading recommendations" });
        }
    }

    [HttpGet("recommendations/{userId:guid}/people")]
    public async Task<IActionResult> GetPeopleRecommendations(Guid userId, int limit = 5)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var recommendations = await _analyticsService.GetPeopleRecommendationsAsync(userId, limit);
            return Ok(new { success = true, data = recommendations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting people recommendations for {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Error loading recommendations" });
        }
    }

    [HttpGet("trending/topics")]
    public async Task<IActionResult> GetTrendingTopics(int days = 7, int limit = 10)
    {
        try
        {
            var topics = await _analyticsService.GetTrendingTopicsAsync(days, limit);
            return Ok(new { success = true, data = topics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending topics");
            return StatusCode(500, new { success = false, message = "Error loading trending topics" });
        }
    }

    [HttpGet("popular/{contentType}")]
    public async Task<IActionResult> GetPopularContent(string contentType, int days = 7, int limit = 10)
    {
        try
        {
            var content = await _analyticsService.GetPopularContentAsync(contentType, days, limit);
            return Ok(new { success = true, data = content });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular content for type {ContentType}", contentType);
            return StatusCode(500, new { success = false, message = "Error loading popular content" });
        }
    }

    [HttpPost("follow/{followedUserId:guid}")]
    public async Task<IActionResult> FollowUser(Guid followedUserId, [FromBody] FollowUserRequest? request = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            if (userId.Value == followedUserId)
            {
                return BadRequest(new { success = false, message = "Cannot follow yourself" });
            }

            var success = await _analyticsService.FollowUserAsync(userId.Value, followedUserId, request?.Reason);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user {FollowedUserId} by user {UserId}", followedUserId, GetCurrentUserId());
            return StatusCode(500, new { success = false, message = "Error following user" });
        }
    }

    [HttpDelete("follow/{followedUserId:guid}")]
    public async Task<IActionResult> UnfollowUser(Guid followedUserId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var success = await _analyticsService.UnfollowUserAsync(userId.Value, followedUserId);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing user {FollowedUserId} by user {UserId}", followedUserId, GetCurrentUserId());
            return StatusCode(500, new { success = false, message = "Error unfollowing user" });
        }
    }

    [HttpGet("following/{userId:guid}")]
    public async Task<IActionResult> GetUserFollowing(Guid userId, bool activeOnly = true)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var following = await _analyticsService.GetUserFollowingAsync(userId, activeOnly);
            return Ok(new { success = true, data = following });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user following for {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Error loading following" });
        }
    }

    [HttpGet("followers/{userId:guid}")]
    public async Task<IActionResult> GetUserFollowers(Guid userId, bool activeOnly = true)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var followers = await _analyticsService.GetUserFollowersAsync(userId, activeOnly);
            return Ok(new { success = true, data = followers });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user followers for {UserId}", userId);
            return StatusCode(500, new { success = false, message = "Error loading followers" });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    public class TrackViewRequest
    {
        public string EntityType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string? EntityTitle { get; set; }
        public int? Duration { get; set; }
    }

    public class TrackInteractionRequest
    {
        public string InteractionType { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string? Metadata { get; set; }
    }

    public class FollowUserRequest
    {
        public string? Reason { get; set; }
    }
}