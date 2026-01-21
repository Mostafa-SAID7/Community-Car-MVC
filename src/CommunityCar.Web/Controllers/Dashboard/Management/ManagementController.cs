using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard.Management;

[Route("dashboard/management")]
public class ManagementController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
