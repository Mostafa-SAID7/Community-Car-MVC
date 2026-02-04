using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community.Interactions;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Features.Shared.Interactions.ViewModels;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Domain.Entities.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using CommunityCar.Infrastructure.Hubs;

namespace CommunityCar.Web.Controllers.Shared.Comments;

[Route("Comments")]
public class CommentsController : Controller
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICommentRepository _commentRepository;
    private readonly UserManager<User> _userManager;
    private readonly IHubContext<NotificationHub> _hubContext;

    public CommentsController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService,
        ICommentRepository commentRepository,
        UserManager<User> userManager,
        IHubContext<NotificationHub> hubContext)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _commentRepository = commentRepository;
        _userManager = userManager;
        _hubContext = hubContext;
    }

    [HttpPost("api/add")]
    public async Task<IActionResult> AddApi([FromBody] CreateCommentVM request)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                return Unauthorized(new { success = false, message = "User not logged in" });

            request.AuthorId = userId;
            var comment = await _interactionService.AddCommentAsync(request);

            // Broadcast via SignalR
            var groupName = $"{request.EntityType}_{request.EntityId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveComment", new 
            {
                id = comment.Id,
                entityId = request.EntityId,
                entityType = request.EntityType,
                content = comment.Content,
                authorName = comment.AuthorName,
                authorAvatar = comment.AuthorAvatar ?? "/images/default-avatar.png",
                timeAgo = "Just now",
                parentCommentId = comment.ParentCommentId
            });

            return Json(new { success = true, comment });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("get/{entityId}")]
    public async Task<IActionResult> GetApi(Guid entityId, [FromQuery] string entityType, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
             if (!Enum.TryParse<EntityType>(entityType, true, out var parsedEntityType))
                 parsedEntityType = EntityType.Post; // Default? Or fail

            var comments = await _interactionService.GetEntityCommentsAsync(entityId, parsedEntityType, page, pageSize);
            return Json(new { success = true, data = comments });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string entityId, string entityType, int page = 1, int pageSize = 10)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
            {
                TempData["ErrorMessage"] = "Invalid entity ID";
                return RedirectToAction("Index", "Home");
            }

            if (!Enum.TryParse<EntityType>(entityType, true, out var parsedEntityType))
            {
                TempData["ErrorMessage"] = "Invalid entity type";
                return RedirectToAction("Index", "Home");
            }

            var comments = await _interactionService.GetEntityCommentsAsync(parsedEntityId, parsedEntityType, page, pageSize);
            var totalCount = await _interactionService.GetTotalTopLevelCommentCountAsync(parsedEntityId, parsedEntityType);
            
            ViewBag.EntityId = parsedEntityId;
            ViewBag.EntityType = parsedEntityType;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.HasMore = totalCount > (page * pageSize);
            
            return View(comments ?? new List<CommunityCar.Application.Features.Shared.Interactions.ViewModels.CommentVM>());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost("Add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(string entityId, string entityType, string content, Guid? parentCommentId = null, string? returnUrl = null)
    {
        try
        {
            // Check if user is authenticated
            if (!User.Identity?.IsAuthenticated == true)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "You must be logged in to comment" });
                
                TempData["ErrorMessage"] = "You must be logged in to comment";
                return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? Request.Headers["Referer"].FirstOrDefault() ?? "/" });
            }

            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Unable to identify user" });
                
                TempData["ErrorMessage"] = "Unable to identify user";
                return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? Request.Headers["Referer"].ToString() });
            }

            if (!Guid.TryParse(entityId, out var parsedEntityId))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Invalid entity ID" });

                TempData["ErrorMessage"] = "Invalid entity ID";
                return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
            }

            if (!Enum.TryParse<EntityType>(entityType, true, out var parsedEntityType))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Invalid entity type" });

                TempData["ErrorMessage"] = "Invalid entity type";
                return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Comment content is required" });

                TempData["ErrorMessage"] = "Comment content is required";
                return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
            }

            var request = new CreateCommentVM
            {
                EntityId = parsedEntityId,
                EntityType = parsedEntityType,
                Content = content.Trim(),
                AuthorId = userId,
                ParentCommentId = parentCommentId
            };

            var comment = await _interactionService.AddCommentAsync(request);

            // Broadcast via SignalR for real-time updates
            var groupName = $"{parsedEntityType}_{parsedEntityId}";
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveComment", new 
            {
                id = comment.Id,
                entityId = parsedEntityId,
                entityType = parsedEntityType,
                content = comment.Content,
                authorName = comment.AuthorName,
                authorAvatar = comment.AuthorAvatar ?? "/images/default-avatar.png",
                timeAgo = "Just now",
                parentCommentId = comment.ParentCommentId
            });
            
            // Handle AJAX requests
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { 
                    success = true, 
                    comment = new {
                        id = comment.Id,
                        content = comment.Content,
                        authorName = comment.AuthorName,
                        authorAvatar = comment.AuthorAvatar ?? "/images/default-avatar.png",
                        timeAgo = "Just now"
                    },
                    message = "Comment posted successfully"
                });
            }
            
            TempData["SuccessMessage"] = "Comment posted successfully";
            return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[COMMENT ERROR]: {ex.Message}");
            Console.WriteLine(ex.StackTrace);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, message = ex.Message });

            TempData["ErrorMessage"] = ex.Message;
            return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
        }
    }

    [HttpGet("GetPartial")]
    public async Task<IActionResult> GetPartial(string entityId, string entityType, int page = 1, int pageSize = 10)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, true, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            var comments = await _interactionService.GetEntityCommentsAsync(parsedEntityId, parsedEntityType, page, pageSize);
            var totalCount = await _interactionService.GetTotalTopLevelCommentCountAsync(parsedEntityId, parsedEntityType);
            
            ViewBag.EntityId = parsedEntityId;
            ViewBag.EntityType = parsedEntityType;
            ViewBag.Page = page;
            
            var hasMore = totalCount > (page * pageSize);
            Response.Headers.Append("X-Has-More", hasMore.ToString().ToLower());
            
            return PartialView("_CommentsList", comments ?? new List<CommunityCar.Application.Features.Shared.Interactions.ViewModels.CommentVM>());
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("Delete")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? returnUrl = null)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                TempData["ErrorMessage"] = "You must be logged in to delete comments";
                return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? Request.Headers["Referer"].FirstOrDefault() ?? "/" });
            }

            var success = await _interactionService.DeleteCommentAsync(id, userId);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (success)
                    return Json(new { success = true, message = "Comment deleted successfully" });
                else
                    return Json(new { success = false, message = "Failed to delete comment" });
            }

            if (success)
                TempData["SuccessMessage"] = "Comment deleted successfully";
            else
                TempData["ErrorMessage"] = "Failed to delete comment";

            return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
        }
        catch (Exception ex)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, message = ex.Message });

            TempData["ErrorMessage"] = ex.Message;
            return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
        }
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "Just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes}m ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours}h ago";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays}d ago";
        if (timeSpan.TotalDays < 30)
            return $"{(int)(timeSpan.TotalDays / 7)}w ago";
        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)}mo ago";
        
        return $"{(int)(timeSpan.TotalDays / 365)}y ago";
    }

    [HttpGet("SearchUsers")]
    public async Task<IActionResult> SearchUsers(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Json(new List<object>());

        var users = await _userManager.Users
            .Where(u => (u.Profile.FullName != null && u.Profile.FullName.Contains(query)) || (u.UserName != null && u.UserName.Contains(query)))
            .Take(5)
            .Select(u => new
            {
                id = u.Id,
                name = u.Profile.FullName ?? u.UserName ?? "Unknown User",
                avatar = u.Profile.ProfilePictureUrl ?? "/images/default-avatar.png"
            })
            .ToListAsync();

        return Json(users);
    }
}


