using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Reports;

[Route("dashboard/reports")]
public class ReportsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
