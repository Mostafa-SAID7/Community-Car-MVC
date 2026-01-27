using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Communication;
using CommunityCar.Application.Features.Chat.DTOs;
using System.Security.Claims;

namespace CommunityCar.Web.Controllers.Chats;

[Route("chats")]
[Authorize]
public class ChatsController : Controller
{
    private readonly IChatService _chatService;

    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? user = null)
    {
        var userId = GetCurrentUserId();
        var conversations = await _chatService.GetUserConversationsAsync(userId);
        
        ViewBag.TargetUserId = user;
        ViewBag.Conversations = conversations;
        
        return View();
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = GetCurrentUserId();
        var conversations = await _chatService.GetUserConversationsAsync(userId);
        return Json(conversations);
    }

    [HttpGet("conversations/{conversationId}")]
    public async Task<IActionResult> GetConversation(Guid conversationId)
    {
        var userId = GetCurrentUserId();
        var conversation = await _chatService.GetConversationAsync(conversationId, userId);
        
        if (conversation == null)
            return NotFound();
            
        return Json(conversation);
    }

    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<IActionResult> GetMessages(Guid conversationId, int page = 1, int pageSize = 50)
    {
        var userId = GetCurrentUserId();
        var messages = await _chatService.GetConversationMessagesAsync(conversationId, userId, page, pageSize);
        return Json(messages);
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
    {
        request.CreatedBy = GetCurrentUserId();
        var conversation = await _chatService.CreateConversationAsync(request);
        return Json(conversation);
    }

    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        request.SenderId = GetCurrentUserId();
        var message = await _chatService.SendMessageAsync(request);
        return Json(message);
    }

    [HttpPost("messages/{messageId}/read")]
    public async Task<IActionResult> MarkMessageAsRead(Guid messageId)
    {
        var userId = GetCurrentUserId();
        var result = await _chatService.MarkMessageAsReadAsync(messageId, userId);
        return Json(new { success = result });
    }

    [HttpPost("conversations/{conversationId}/read")]
    public async Task<IActionResult> MarkConversationAsRead(Guid conversationId)
    {
        var userId = GetCurrentUserId();
        var result = await _chatService.MarkConversationAsReadAsync(conversationId, userId);
        return Json(new { success = result });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = GetCurrentUserId();
        var count = await _chatService.GetUnreadMessageCountAsync(userId);
        return Json(new { count });
    }

    [HttpPost("users/online-status")]
    public async Task<IActionResult> GetOnlineStatus([FromBody] List<Guid> userIds)
    {
        var statuses = await _chatService.GetOnlineUsersAsync(userIds);
        return Json(statuses);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }
}
