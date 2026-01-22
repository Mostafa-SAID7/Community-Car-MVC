using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;

namespace CommunityCar.Web.Controllers.Shared;

[Authorize]
public class InteractionsController : Controller
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<InteractionsController> _logger;

    public InteractionsController(
        IInteractionService interactionService, 
        ICurrentUserService currentUserService,
        ILogger<InteractionsController> logger)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Display interactions management page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Display interaction history for the current user
    /// </summary>
    public IActionResult History()
    {
        return View();
    }

    /// <summary>
    /// Display interaction settings page
    /// </summary>
    public IActionResult Settings()
    {
        return View();
    }
}