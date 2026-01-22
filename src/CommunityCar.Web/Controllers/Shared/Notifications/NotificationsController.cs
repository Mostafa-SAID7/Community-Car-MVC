using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Common.Interfaces.Services.Identity;

namespace CommunityCar.Web.Controllers.Shared.Notifications;

[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ICurrentUserService currentUserService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Display the notifications page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Display notification settings page
    /// </summary>
    public IActionResult Settings()
    {
        return View();
    }

    /// <summary>
    /// Display notification preferences page
    /// </summary>
    public IActionResult Preferences()
    {
        return View();
    }
}