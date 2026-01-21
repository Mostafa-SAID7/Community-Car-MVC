using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Community.Stories;

[Route("stories")]
public class StoriesController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Community/Stories/Index.cshtml");
    }
}
