using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers;

[Authorize]
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
            
        var summary = await _interactionService.GetInteractionSummaryAsync(testEntityId, EntityType.Question, userId);
        
        ViewBag.EntityId = testEntityId;
        ViewBag.EntityType = (int)EntityType.Question;
        
        return View(summary);
    }
}