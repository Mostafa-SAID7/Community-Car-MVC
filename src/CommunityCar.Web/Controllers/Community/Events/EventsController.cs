using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Community.Events;

[Route("events")]
public class EventsController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Community/Events/Index.cshtml");
    }
}
