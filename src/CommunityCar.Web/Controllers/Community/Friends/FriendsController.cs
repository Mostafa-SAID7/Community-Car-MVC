using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Community.Friends;

[Route("friends")]
public class FriendsController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Community/Friends/Index.cshtml");
    }
}
