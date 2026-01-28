using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Shared;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Features.Shared.DTOs;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Web.Controllers.Shared.Search;

[Route("shared/search")]
[Controller]
public class SharedSearchController : Controller
{
    private readonly ISharedSearchService _searchService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SharedSearchController> _logger;

    public SharedSearchController(
        ISharedSearchService searchService,
        ICurrentUserService currentUserService,
        ILogger<SharedSearchController> logger)
    {
        _searchService = searchService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Universal search across all shared entities
    /// </summary>
    [HttpGet("universal")]
    public async Task<IActionResult> UniversalSearch([FromQuery] UniversalSearchRequest request)
    {
        try
        {
            if (Guid.TryParse(_currentUserService.UserId, out var userId))
                request.UserId = userId;

            var result = await _searchService.SearchAllAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing universal search");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Search bookmarks
    /// </summary>
    [HttpGet("bookmarks")]
    [Authorize]
    public async Task<IActionResult> SearchBookmarks([FromQuery] BookmarkSearchRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            request.UserId = userId;
            var result = await _searchService.SearchBookmarksAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookmarks");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Search comments
    /// </summary>
    [HttpGet("comments")]
    public async Task<IActionResult> SearchComments([FromQuery] CommentSearchRequest request)
    {
        try
        {
            if (Guid.TryParse(_currentUserService.UserId, out var userId))
                request.UserId = userId;

            var result = await _searchService.SearchCommentsAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching comments");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Search categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> SearchCategories([FromQuery] CategorySearchRequest request)
    {
        try
        {
            var result = await _searchService.SearchCategoriesAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching categories");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Search tags
    /// </summary>
    [HttpGet("tags")]
    public async Task<IActionResult> SearchTags([FromQuery] TagSearchRequest request)
    {
        try
        {
            var result = await _searchService.SearchTagsAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tags");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Search reactions
    /// </summary>
    [HttpGet("reactions")]
    public async Task<IActionResult> SearchReactions([FromQuery] ReactionSearchRequest request)
    {
        try
        {
            if (Guid.TryParse(_currentUserService.UserId, out var userId))
                request.UserId = userId;

            var result = await _searchService.SearchReactionsAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching reactions");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Search shares
    /// </summary>
    [HttpGet("shares")]
    public async Task<IActionResult> SearchShares([FromQuery] ShareSearchRequest request)
    {
        try
        {
            if (Guid.TryParse(_currentUserService.UserId, out var userId))
                request.UserId = userId;

            var result = await _searchService.SearchSharesAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching shares");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Search ratings
    /// </summary>
    [HttpGet("ratings")]
    public async Task<IActionResult> SearchRatings([FromQuery] RatingSearchRequest request)
    {
        try
        {
            if (Guid.TryParse(_currentUserService.UserId, out var userId))
                request.UserId = userId;

            var result = await _searchService.SearchRatingsAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching ratings");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Search votes
    /// </summary>
    [HttpGet("votes")]
    public async Task<IActionResult> SearchVotes([FromQuery] VoteSearchRequest request)
    {
        try
        {
            if (Guid.TryParse(_currentUserService.UserId, out var userId))
                request.UserId = userId;

            var result = await _searchService.SearchVotesAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching votes");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Search views
    /// </summary>
    [HttpGet("views")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> SearchViews([FromQuery] ViewSearchRequest request)
    {
        try
        {
            var result = await _searchService.SearchViewsAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching views");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get search suggestions
    /// </summary>
    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions([FromQuery] string query, [FromQuery] int maxResults = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { success = false, message = "Query parameter is required" });

            var suggestions = await _searchService.GetSearchSuggestionsAsync(query, maxResults);
            return Ok(new { success = true, data = suggestions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search suggestions");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get trending items
    /// </summary>
    [HttpGet("trending")]
    public async Task<IActionResult> GetTrending([FromQuery] EntityType? entityType = null, [FromQuery] int maxResults = 20)
    {
        try
        {
            var trending = await _searchService.GetTrendingItemsAsync(entityType, maxResults);
            return Ok(new { success = true, data = trending });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending items");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get search analytics
    /// </summary>
    [HttpGet("analytics")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> GetAnalytics()
    {
        try
        {
            Guid? userId = null;
            if (Guid.TryParse(_currentUserService.UserId, out var parsedUserId))
                userId = parsedUserId;

            var analytics = await _searchService.GetSearchAnalyticsAsync(userId);
            return Ok(new { success = true, data = analytics });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search analytics");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get related content
    /// </summary>
    [HttpGet("related/{entityType}/{entityId}")]
    public async Task<IActionResult> GetRelatedContent(EntityType entityType, Guid entityId, [FromQuery] int maxResults = 10)
    {
        try
        {
            var relatedContent = await _searchService.GetRelatedContentAsync(entityId, entityType, maxResults);
            return Ok(new { success = true, data = relatedContent });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting related content");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get popular content
    /// </summary>
    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularContent(
        [FromQuery] EntityType? entityType = null, 
        [FromQuery] int days = 7, 
        [FromQuery] int maxResults = 20)
    {
        try
        {
            var timeRange = TimeSpan.FromDays(days);
            var popularContent = await _searchService.GetPopularContentAsync(entityType, timeRange, maxResults);
            return Ok(new { success = true, data = popularContent });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular content");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Advanced search with multiple filters
    /// </summary>
    [HttpPost("advanced")]
    public async Task<IActionResult> AdvancedSearch([FromBody] UniversalSearchRequest request)
    {
        try
        {
            if (Guid.TryParse(_currentUserService.UserId, out var userId))
                request.UserId = userId;

            var result = await _searchService.SearchAllAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing advanced search");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}


