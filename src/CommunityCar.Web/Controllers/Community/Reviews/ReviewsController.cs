using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Community.Reviews;

[Route("reviews")]
public class ReviewsController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Community/Reviews/Index.cshtml");
    }
}
