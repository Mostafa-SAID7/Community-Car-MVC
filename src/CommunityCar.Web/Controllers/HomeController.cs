using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Redirect to the feed URL with default culture to avoid infinite loop
        return Redirect("/en/feed");
    }
}


