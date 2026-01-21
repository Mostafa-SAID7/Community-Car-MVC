using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CommunityCar.Web.Controllers;

[Authorize]
public class TestController : Controller
{
    public IActionResult SignalR()
    {
        return View();
    }
}