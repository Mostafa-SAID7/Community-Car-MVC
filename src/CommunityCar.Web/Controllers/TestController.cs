using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers;

[Authorize]
public class TestController : Controller
{
    private readonly IInteractionService _interactionService;

    public TestController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    public IActionResult SignalR()
    {
        return View();
    }

    public async Task<IActionResult> Interactions()
    {
        // Create a test interaction summary
        var testEntityId = Guid.NewGuid();
        var summary = await _interactionService.GetInteractionSummaryAsync(testEntityId, EntityType.Question, User.Identity?.IsAuthenticated == true ? Guid.NewGuid() : null);
        
        ViewBag.EntityId = testEntityId;
        ViewBag.EntityType = (int)EntityType.Question;
        
        return View(summary);
    }
}