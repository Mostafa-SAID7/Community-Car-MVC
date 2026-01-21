using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Analytics;

[Route("dashboard/analytics")]
public class AnalyticsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
