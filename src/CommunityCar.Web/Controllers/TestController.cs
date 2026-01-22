using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers;

// [Authorize] - Temporarily disabled for testing
public class TestController : Controller
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;

    public TestController(IInteractionService interactionService, ICurrentUserService currentUserService)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
    }

    public IActionResult SignalR()
    {
        return View();
    }

    public async Task<IActionResult> Interactions()
    {
        // Create a test interaction summary
        var testEntityId = Guid.NewGuid();
        
        Guid? userId = null;
        var userIdString = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
            userId = parsedUserId;
            
        // For testing without authentication, create a mock summary
        var summary = new CommunityCar.Application.Features.Interactions.ViewModels.InteractionSummaryVM
        {
            Reactions = new CommunityCar.Application.Features.Interactions.ViewModels.ReactionSummaryVM
            {
                TotalReactions = 0,
                ReactionCounts = new Dictionary<CommunityCar.Domain.Enums.ReactionType, int>(),
                AvailableReactions = new List<CommunityCar.Application.Features.Interactions.ViewModels.ReactionTypeInfoVM>
                {
                    new() { Type = CommunityCar.Domain.Enums.ReactionType.Like, Display = "Like", Icon = "fas fa-thumbs-up", Count = 0 },
                    new() { Type = CommunityCar.Domain.Enums.ReactionType.Love, Display = "Love", Icon = "fas fa-heart", Count = 0 },
                    new() { Type = CommunityCar.Domain.Enums.ReactionType.Haha, Display = "Haha", Icon = "fas fa-laugh", Count = 0 }
                }
            },
            CommentCount = 0,
            Shares = new CommunityCar.Application.Features.Interactions.ViewModels.ShareSummaryVM
            {
                TotalShares = 0,
                ShareTypeCounts = new Dictionary<CommunityCar.Domain.Enums.ShareType, int>()
            },
            CanComment = userId.HasValue,
            CanShare = userId.HasValue,
            CanReact = userId.HasValue
        };
        
        ViewBag.EntityId = testEntityId;
        ViewBag.EntityType = (int)CommunityCar.Domain.Enums.EntityType.Question;
        
        return View(summary);
    }
}