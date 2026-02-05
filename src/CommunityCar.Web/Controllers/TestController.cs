using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community.Interactions;
using CommunityCar.Web.Areas.Identity.Interfaces.Services.Core;
using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Domain.Entities.Community.News;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Web.Controllers;

// [Authorize] - Temporarily disabled for testing
public class TestController : Controller
{
    private readonly IInteractionService _interactionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;

    public TestController(IInteractionService interactionService, ICurrentUserService currentUserService, IApplicationDbContext context)
    {
        _interactionService = interactionService;
        _currentUserService = currentUserService;
        _context = context;
    }

    public IActionResult SignalR()
    {
        return View();
    }

    public IActionResult Interactions()
    {
        // Create a test interaction summary
        var testEntityId = Guid.NewGuid();
        
        Guid? userId = null;
        var userIdString = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var parsedUserId))
            userId = parsedUserId;
            
        // For testing without authentication, create a mock summary
        var summary = new CommunityCar.Application.Features.Shared.Interactions.ViewModels.InteractionSummaryVM
        {
            Reactions = new CommunityCar.Application.Features.Shared.Interactions.ViewModels.ReactionSummaryVM
            {
                TotalReactions = 0,
                ReactionCounts = new Dictionary<ReactionType, int>(),
                AvailableReactions = new List<CommunityCar.Application.Features.Shared.Interactions.ViewModels.ReactionTypeInfoVM>
                {
                    new() { Type = ReactionType.Like, Display = "Like", Icon = "fas fa-thumbs-up", Count = 0 },
                    new() { Type = ReactionType.Love, Display = "Love", Icon = "fas fa-heart", Count = 0 },
                    new() { Type = ReactionType.Haha, Display = "Haha", Icon = "fas fa-laugh", Count = 0 }
                }
            },
            CommentCount = 0,
            Shares = new CommunityCar.Application.Features.Shared.Interactions.ViewModels.ShareSummaryVM
            {
                TotalShares = 0,
                ShareTypeCounts = new Dictionary<ShareType, int>()
            },
            CanComment = userId.HasValue,
            CanShare = userId.HasValue,
            CanReact = userId.HasValue
        };
        
        ViewBag.EntityId = testEntityId;
        ViewBag.EntityType = (int)EntityType.Question;
        
        return View(summary);
    }

    [HttpGet]
    public async Task<IActionResult> SoftDelete()
    {
        try
        {
            // Create a test news item
            var testNews = new NewsItem(
                "Test News for Soft Delete",
                "This is a test news item to verify soft delete functionality.",
                Guid.NewGuid(),
                NewsCategory.General
            );
            testNews.AddTag("test");
            testNews.AddTag("soft-delete");

            _context.News.Add(testNews);
            await _context.SaveChangesAsync();

            // Verify it exists
            var existsBeforeDelete = await _context.News.AnyAsync(n => n.Id == testNews.Id);

            // Soft delete it
            testNews.SoftDelete("TestUser");
            await _context.SaveChangesAsync();

            // Verify it's soft deleted (should not appear in normal queries)
            var existsAfterDelete = await _context.News.AnyAsync(n => n.Id == testNews.Id);

            // Verify it still exists when including deleted items
            var existsWithDeleted = await _context.News.IgnoreQueryFilters().AnyAsync(n => n.Id == testNews.Id);

            // Restore it
            testNews.Restore("TestUser");
            await _context.SaveChangesAsync();

            // Verify it's restored
            var existsAfterRestore = await _context.News.AnyAsync(n => n.Id == testNews.Id);

            // Clean up
            _context.News.Remove(testNews);
            await _context.SaveChangesAsync();

            var result = new
            {
                TestNewsId = testNews.Id,
                ExistsBeforeDelete = existsBeforeDelete,
                ExistsAfterDelete = existsAfterDelete,
                ExistsWithDeleted = existsWithDeleted,
                ExistsAfterRestore = existsAfterRestore,
                Success = existsBeforeDelete && !existsAfterDelete && existsWithDeleted && existsAfterRestore
            };

            return Json(result);
        }
        catch (Exception ex)
        {
            return Json(new { Error = ex.Message, Success = false });
        }
    }

    [HttpGet("test-error")]
    public IActionResult TestError()
    {
        // This will trigger an error for testing the error reporting system
        throw new InvalidOperationException("This is a test error for demonstrating the error reporting system. The profile badges view could not be found.");
    }

    [HttpGet("test-not-found")]
    public IActionResult TestNotFound()
    {
        return NotFound("This is a test 404 error for testing the error reporting system.");
    }

    [HttpGet("test-unauthorized")]
    public IActionResult TestUnauthorized()
    {
        throw new UnauthorizedAccessException("This is a test unauthorized access error.");
    }
    
    [HttpGet("error-reporting")]
    public IActionResult ErrorReporting()
    {
        return View();
    }
    
    [HttpGet("automapper-test")]
    public IActionResult AutoMapperTest()
    {
        return View();
    }
    
    [HttpGet("qa-debug")]
    public async Task<IActionResult> QADebug()
    {
        try
        {
            // Check if questions exist in database
            var questionsCount = await _context.Questions.CountAsync();
            var questions = await _context.Questions.Take(5).ToListAsync();
            
            var result = new
            {
                QuestionsCount = questionsCount,
                Questions = questions.Select(q => new
                {
                    q.Id,
                    q.Title,
                    q.Slug,
                    q.CreatedAt,
                    q.AuthorId
                }).ToList(),
                Success = true
            };
            
            return Json(result);
        }
        catch (Exception ex)
        {
            return Json(new { Error = ex.Message, Success = false });
        }
    }
}



