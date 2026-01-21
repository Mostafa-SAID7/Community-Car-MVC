using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Monitoring;

[Route("dashboard/monitoring")]
public class MonitoringController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
