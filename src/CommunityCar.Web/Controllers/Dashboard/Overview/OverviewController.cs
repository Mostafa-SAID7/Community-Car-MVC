using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Overview;

[Route("dashboard/overview")]
public class OverviewController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Dashboard/Overview/Index.cshtml");
    }
}
