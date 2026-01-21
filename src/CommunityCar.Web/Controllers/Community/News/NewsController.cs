using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Community.News;

[Route("news")]
public class NewsController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Community/News/Index.cshtml");
    }
}
