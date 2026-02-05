using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community.Interactions;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Web.Controllers.Shared.Ratings;

[Route("shared/ratings")]
[Controller]
public class RatingsController : Controller
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRatingRepository _ratingRepository;

    public RatingsController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService,
        IRatingRepository ratingRepository)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _ratingRepository = ratingRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddRating([FromBody] CreateRatingRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var rating = new Rating(request.EntityId, request.EntityType, userId, (int)request.Value, request.Comment);
            await _ratingRepository.AddAsync(rating);
            
            return Ok(new { success = true, rating });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateRating(Guid id, [FromBody] UpdateRatingRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var rating = await _ratingRepository.GetByIdAsync(id);
            if (rating == null)
                return NotFound(new { success = false, message = "Rating not found" });

            if (rating.UserId != userId)
                return Forbid("You can only update your own ratings");

            // Note: Rating entity would need update methods
            await _ratingRepository.UpdateAsync(rating);
            return Ok(new { success = true, rating });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}")]
    public async Task<IActionResult> GetEntityRatings(EntityType entityType, Guid entityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var ratings = await _ratingRepository.GetEntityRatingsAsync(entityId, entityType, page, pageSize);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/average")]
    public async Task<IActionResult> GetAverageRating(EntityType entityType, Guid entityId)
    {
        try
        {
            var average = await _ratingRepository.GetAverageRatingAsync(entityId, entityType);
            var count = await _ratingRepository.GetRatingCountAsync(entityId, entityType);
            
            return Ok(new { averageRating = average, ratingCount = count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/distribution")]
    public async Task<IActionResult> GetRatingDistribution(EntityType entityType, Guid entityId)
    {
        try
        {
            var distribution = await _ratingRepository.GetRatingDistributionAsync(entityId, entityType);
            return Ok(distribution);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/check")]
    [Authorize]
    public async Task<IActionResult> CheckUserRating(EntityType entityType, Guid entityId)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var rating = await _ratingRepository.GetUserRatingAsync(entityId, entityType, userId);
            return Ok(new { 
                hasRated = rating != null, 
                rating = rating?.Value,
                ratingId = rating?.Id 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteRating(Guid id)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var rating = await _ratingRepository.GetByIdAsync(id);
            if (rating == null)
                return NotFound(new { success = false, message = "Rating not found" });

            if (rating.UserId != userId)
                return Forbid("You can only delete your own ratings");

            await _ratingRepository.DeleteAsync(rating);
            return Ok(new { success = true, message = "Rating deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetUserRatings([FromQuery] EntityType? entityType = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var ratings = await _ratingRepository.GetUserRatingsAsync(userId, entityType);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreateRatingRequest
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public double Value { get; set; }
    public string? Comment { get; set; }
}

public class UpdateRatingRequest
{
    public double Value { get; set; }
    public string? Comment { get; set; }
}



