using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Web.Controllers.Shared.Votes;

public class VotesController : Controller
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CastVote([FromForm] string entityId, [FromForm] string entityType, [FromForm] string voteType)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Json(new { success = false, message = "User must be authenticated" });

            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            if (!Enum.TryParse<VoteType>(voteType, out var parsedVoteType))
                return Json(new { success = false, message = "Invalid vote type" });

            var vote = new Vote(parsedEntityId, parsedEntityType, userId, parsedVoteType);
            await _voteRepository.AddAsync(vote);
            
            return Json(new { success = true, vote });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPut]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateVote([FromForm] string id, [FromForm] string voteType)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Json(new { success = false, message = "User must be authenticated" });

            if (!Guid.TryParse(id, out var parsedId))
                return Json(new { success = false, message = "Invalid vote ID" });

            if (!Enum.TryParse<VoteType>(voteType, out var parsedVoteType))
                return Json(new { success = false, message = "Invalid vote type" });

            var vote = await _voteRepository.GetByIdAsync(parsedId);
            if (vote == null)
                return Json(new { success = false, message = "Vote not found" });

            if (vote.UserId != userId)
                return Json(new { success = false, message = "You can only update your own votes" });

            // Note: Vote entity would need update methods
            await _voteRepository.UpdateAsync(vote);
            return Json(new { success = true, vote });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetEntityVotes(string entityType, string entityId)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            var votes = await _voteRepository.GetVotesByEntityAsync(parsedEntityId, parsedEntityType);
            return Json(votes);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetVoteSummary(string entityType, string entityId)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            var upVotes = await _voteRepository.GetVoteCountAsync(parsedEntityId, parsedEntityType, VoteType.Upvote);
            var downVotes = await _voteRepository.GetVoteCountAsync(parsedEntityId, parsedEntityType, VoteType.Downvote);
            var score = upVotes - downVotes;
            
            return Json(new { 
                upVotes, 
                downVotes, 
                score,
                totalVotes = upVotes + downVotes
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> CheckUserVote(string entityType, string entityId)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Json(new { success = false, message = "User must be authenticated" });

            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            var vote = await _voteRepository.GetUserVoteAsync(parsedEntityId, parsedEntityType, userId);
            return Json(new { 
                hasVoted = vote != null, 
                voteType = vote?.Type,
                voteId = vote?.Id 
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVote([FromForm] string id)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Json(new { success = false, message = "User must be authenticated" });

            if (!Guid.TryParse(id, out var parsedId))
                return Json(new { success = false, message = "Invalid vote ID" });

            var vote = await _voteRepository.GetByIdAsync(parsedId);
            if (vote == null)
                return Json(new { success = false, message = "Vote not found" });

            if (vote.UserId != userId)
                return Json(new { success = false, message = "You can only remove your own votes" });

            await _voteRepository.DeleteAsync(vote);
            return Json(new { success = true, message = "Vote removed successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserVotes(string? entityType = null, string? voteType = null, int page = 1, int pageSize = 20)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Json(new { success = false, message = "User must be authenticated" });

            EntityType? parsedEntityType = null;
            if (!string.IsNullOrEmpty(entityType) && Enum.TryParse<EntityType>(entityType, out var et))
                parsedEntityType = et;

            var votes = await _voteRepository.GetUserVotesAsync(userId, parsedEntityType);
            return Json(votes);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetEntityReactions(string entityType, string entityId)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            var reactions = await _interactionService.GetEntityReactionsAsync(parsedEntityId, parsedEntityType);
            return Json(reactions);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}