using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Application.Features.Interactions.DTOs;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Shared.Comments;

[Route("Comments")]
public class CommentsController : Controller
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICommentRepository _commentRepository;

    public CommentsController(
        IInteractionService interactionService,
        ICurrentUserService currentUserService,
        ICommentRepository commentRepository)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _commentRepository = commentRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string entityId, string entityType, int page = 1, int pageSize = 10)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
            {
                TempData["ErrorMessage"] = "Invalid entity ID";
                return RedirectToAction("Index", "Home");
            }

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
            {
                TempData["ErrorMessage"] = "Invalid entity type";
                return RedirectToAction("Index", "Home");
            }

            var comments = await _interactionService.GetEntityCommentsAsync(parsedEntityId, parsedEntityType, page, pageSize);
            
            ViewBag.EntityId = parsedEntityId;
            ViewBag.EntityType = parsedEntityType;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            
            return View(comments ?? new List<CommunityCar.Application.Features.Interactions.ViewModels.CommentVM>());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string entityId, string entityType, string content, string returnUrl = null)
    {
        try
        {
            // Check if user is authenticated
            if (!User.Identity?.IsAuthenticated == true)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "You must be logged in to comment" });
                
                TempData["ErrorMessage"] = "You must be logged in to comment";
                return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? Request.Headers["Referer"].ToString() });
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
                TempData["ErrorMessage"] = "Invalid entity ID";
                return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
            }

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
            {
                TempData["ErrorMessage"] = "Invalid entity type";
                return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Comment content is required";
                return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
            }

            var request = new CreateCommentRequest
            {
                EntityId = parsedEntityId,
                EntityType = parsedEntityType,
                Content = content.Trim(),
                AuthorId = userId
            };

            var comment = await _interactionService.AddCommentAsync(request);
            
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
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, message = ex.Message });

            TempData["ErrorMessage"] = ex.Message;
            return Redirect(returnUrl ?? Request.Headers["Referer"].ToString());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPartial(string entityId, string entityType, int page = 1, int pageSize = 10)
    {
        try
        {
            if (!Guid.TryParse(entityId, out var parsedEntityId))
                return Json(new { success = false, message = "Invalid entity ID" });

            if (!Enum.TryParse<EntityType>(entityType, out var parsedEntityType))
                return Json(new { success = false, message = "Invalid entity type" });

            var comments = await _interactionService.GetEntityCommentsAsync(parsedEntityId, parsedEntityType, page, pageSize);
            
            ViewBag.EntityId = parsedEntityId;
            ViewBag.EntityType = parsedEntityType;
            
            return PartialView("_CommentsList", comments ?? new List<CommunityCar.Application.Features.Interactions.ViewModels.CommentVM>());
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string returnUrl = null)
    {
        try
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
            {
                TempData["ErrorMessage"] = "You must be logged in to delete comments";
                return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? Request.Headers["Referer"].ToString() });
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
}