using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Settings;

[Route("dashboard/settings")]
public class SettingsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
