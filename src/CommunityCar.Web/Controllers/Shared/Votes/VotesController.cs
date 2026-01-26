using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Web.Controllers.Shared.Votes;

[Route("api/shared/votes")]
[ApiController]
public class VotesController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IVoteRepository _voteRepository;

    public VotesController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService,
        IVoteRepository voteRepository)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _voteRepository = voteRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CastVote([FromBody] CreateVoteRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var vote = new Vote(request.EntityId, request.EntityType, userId, request.VoteType);
            await _voteRepository.AddAsync(vote);
            
            return Ok(new { success = true, vote });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateVote(Guid id, [FromBody] UpdateVoteRequest request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var vote = await _voteRepository.GetByIdAsync(id);
            if (vote == null)
                return NotFound(new { success = false, message = "Vote not found" });

            if (vote.UserId != userId)
                return Forbid("You can only update your own votes");

            // Note: Vote entity would need update methods
            await _voteRepository.UpdateAsync(vote);
            return Ok(new { success = true, vote });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}")]
    public async Task<IActionResult> GetEntityVotes(EntityType entityType, Guid entityId)
    {
        try
        {
            var votes = await _voteRepository.GetVotesByEntityAsync(entityId, entityType);
            return Ok(votes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/summary")]
    public async Task<IActionResult> GetVoteSummary(EntityType entityType, Guid entityId)
    {
        try
        {
            var upVotes = await _voteRepository.GetVoteCountAsync(entityId, entityType, VoteType.Upvote);
            var downVotes = await _voteRepository.GetVoteCountAsync(entityId, entityType, VoteType.Downvote);
            var score = upVotes - downVotes;
            
            return Ok(new { 
                upVotes, 
                downVotes, 
                score,
                totalVotes = upVotes + downVotes
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/check")]
    [Authorize]
    public async Task<IActionResult> CheckUserVote(EntityType entityType, Guid entityId)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var vote = await _voteRepository.GetUserVoteAsync(entityId, entityType, userId);
            return Ok(new { 
                hasVoted = vote != null, 
                voteType = vote?.Type,
                voteId = vote?.Id 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> RemoveVote(Guid id)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var vote = await _voteRepository.GetByIdAsync(id);
            if (vote == null)
                return NotFound(new { success = false, message = "Vote not found" });

            if (vote.UserId != userId)
                return Forbid("You can only remove your own votes");

            await _voteRepository.DeleteAsync(vote);
            return Ok(new { success = true, message = "Vote removed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetUserVotes([FromQuery] EntityType? entityType = null, [FromQuery] VoteType? voteType = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User must be authenticated" });

            var votes = await _voteRepository.GetUserVotesAsync(userId, entityType);
            return Ok(votes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{entityType}/{entityId}/reactions")]
    public async Task<IActionResult> GetEntityReactions(EntityType entityType, Guid entityId)
    {
        try
        {
            var reactions = await _interactionService.GetEntityReactionsAsync(entityId, entityType);
            return Ok(reactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreateVoteRequest
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public VoteType VoteType { get; set; }
}

public class UpdateVoteRequest
{
    public VoteType VoteType { get; set; }
}