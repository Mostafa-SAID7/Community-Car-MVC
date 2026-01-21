using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Chats;

[Route("chats")]
public class ChatsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
