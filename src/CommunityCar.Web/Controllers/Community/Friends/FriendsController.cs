using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Community.Friends.ViewModels;
using CommunityCar.Domain.Entities.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityCar.Web.Controllers.Community.Friends;

[Route("{culture}/friends")]
[Authorize]
public class FriendsController : Controller
{
    private readonly IFriendsService _friendsService;
    private readonly UserManager<User> _userManager;

    public FriendsController(IFriendsService friendsService, UserManager<User> userManager)
    {
        _friendsService = friendsService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var overview = await _friendsService.GetFriendsOverviewAsync(userId);
        return View("~/Views/Community/Friends/Index.cshtml", overview);
    }

    [HttpGet("all")]
    public async Task<IActionResult> All()
    {
        var userId = GetCurrentUserId();
        var friends = await _friendsService.GetFriendsAsync(userId);
        return View("~/Views/Community/Friends/All.cshtml", friends);
    }

    [HttpGet("requests")]
    public async Task<IActionResult> Requests()
    {
        var userId = GetCurrentUserId();
        var pendingRequests = await _friendsService.GetPendingRequestsAsync(userId);
        var sentRequests = await _friendsService.GetSentRequestsAsync(userId);
        
        var model = new FriendRequestsVM
        {
            PendingRequests = pendingRequests,
            SentRequests = sentRequests
        };
        
        return View("~/Views/Community/Friends/Requests.cshtml", model);
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> Suggestions()
    {
        var userId = GetCurrentUserId();
        var suggestions = await _friendsService.GetFriendSuggestionsAsync(userId, 20);
        return View("~/Views/Community/Friends/Suggestions.cshtml", suggestions);
    }

    [HttpPost("send-request")]
    public async Task<IActionResult> SendRequest([FromBody] SendFriendRequestModel model)
    {
        var userId = GetCurrentUserId();
        var result = await _friendsService.SendFriendRequestAsync(userId, model.ReceiverId);
        
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost("accept-request")]
    public async Task<IActionResult> AcceptRequest([FromBody] FriendshipActionModel model)
    {
        var userId = GetCurrentUserId();
        var result = await _friendsService.AcceptFriendRequestAsync(userId, model.FriendshipId);
        
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost("decline-request")]
    public async Task<IActionResult> DeclineRequest([FromBody] FriendshipActionModel model)
    {
        var userId = GetCurrentUserId();
        var result = await _friendsService.DeclineFriendRequestAsync(userId, model.FriendshipId);
        
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost("remove-friend")]
    public async Task<IActionResult> RemoveFriend([FromBody] RemoveFriendModel model)
    {
        var userId = GetCurrentUserId();
        var result = await _friendsService.RemoveFriendAsync(userId, model.FriendId);
        
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost("block-user")]
    public async Task<IActionResult> BlockUser([FromBody] BlockUserModel model)
    {
        var userId = GetCurrentUserId();
        var result = await _friendsService.BlockUserAsync(userId, model.UserToBlockId);
        
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet("mutual/{friendId}")]
    public async Task<IActionResult> MutualFriends(Guid friendId)
    {
        var userId = GetCurrentUserId();
        var mutualFriends = await _friendsService.GetMutualFriendsAsync(userId, friendId);
        return View("~/Views/Community/Friends/MutualFriends.cshtml", mutualFriends);
    }

    [HttpGet("status/{otherUserId}")]
    public async Task<IActionResult> GetFriendshipStatus(Guid otherUserId)
    {
        var userId = GetCurrentUserId();
        var status = await _friendsService.GetFriendshipStatusAsync(userId, otherUserId);
        return Json(status);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }
}

// Request models
public class SendFriendRequestModel
{
    public Guid ReceiverId { get; set; }
}

public class FriendshipActionModel
{
    public Guid FriendshipId { get; set; }
}

public class RemoveFriendModel
{
    public Guid FriendId { get; set; }
}

public class BlockUserModel
{
    public Guid UserToBlockId { get; set; }
}

public class FriendRequestsVM
{
    public IEnumerable<FriendRequestVM> PendingRequests { get; set; } = new List<FriendRequestVM>();
    public IEnumerable<FriendRequestVM> SentRequests { get; set; } = new List<FriendRequestVM>();
}



