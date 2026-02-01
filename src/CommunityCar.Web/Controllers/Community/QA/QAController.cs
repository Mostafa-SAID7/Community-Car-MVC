using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Services.Identity;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Features.QA.ViewModels;
using CommunityCar.Application.Features.QA.DTOs;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Domain.Entities.Community.QA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Community.QA;

[Route("qa")]
public class QAController : Controller
{
    private readonly IQAService _qaService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<QAController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public QAController(IQAService qaService, ICurrentUserService currentUserService, ILogger<QAController> logger, IUnitOfWork unitOfWork)
    {
        _qaService = qaService;
        _currentUserService = currentUserService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] QASearchRequest? request)
    {
        request ??= new QASearchRequest();
        
        // Set default values if not provided
        if (request.Page <= 0) request.Page = 1;
        if (request.PageSize <= 0 || request.PageSize > 100) request.PageSize = 20;
        
        var response = await _qaService.SearchQuestionsAsync(request);
        
        ViewBag.SearchRequest = request;
        ViewBag.SearchResponse = response;

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("~/Views/Community/QA/_QuestionList.cshtml", response);
        }
        
        return View("~/Views/Community/QA/Index.cshtml", response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] QASearchRequest request)
    {
        var response = await _qaService.SearchQuestionsAsync(request);
        return View("~/Views/Community/QA/SearchResults.cshtml", response);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(string slug)
    {
        var question = await _qaService.GetQuestionBySlugAsync(slug);
        if (question == null) return NotFound();

        var id = question.Id;
        // Increment view count
        var userId = _currentUserService.UserId != null ? Guid.Parse(_currentUserService.UserId) : (Guid?)null;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        
        await _qaService.IncrementViewCountAsync(id, userId, ipAddress, userAgent);

        ViewBag.Answers = await _qaService.GetAnswersByQuestionIdAsync(id);
        return View("~/Views/Community/QA/Details.cshtml", question);
    }

    [HttpGet("ask")]
    [Authorize]
    public IActionResult Create()
    {
        return View("~/Views/Community/QA/Create.cshtml");
    }

    [HttpPost("ask")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(CreateQuestionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Community/QA/Create.cshtml", request);
        }

        var authorId = Guid.Parse(_currentUserService.UserId!);
        
        await _qaService.CreateQuestionAsync(
            request.Title, 
            request.Body, 
            authorId, 
            request.TitleAr,
            request.BodyAr,
            request.Difficulty,
            request.CarMake,
            request.CarModel,
            request.CarYear,
            request.CarEngine,
            request.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList()
        );
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:guid}/answer")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Answer(Guid id, string body, string? bodyAr = null)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            var q = await _qaService.GetQuestionByIdAsync(id);
            return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? id.ToString() });
        }

        var authorId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.CreateAnswerAsync(id, body, authorId, bodyAr);

        var question = await _qaService.GetQuestionByIdAsync(id);
        return RedirectToAction(nameof(Details), new { slug = question?.Slug ?? id.ToString() });
    }

    [HttpPost("answer/{answerId:guid}/accept")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AcceptAnswer(Guid answerId, Guid questionId)
    {
        // TODO: Check if current user is the author of the question
        await _qaService.AcceptAnswerAsync(answerId);
        var q = await _qaService.GetQuestionByIdAsync(questionId);
        return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? questionId.ToString() });
    }

    // Alternative route for AcceptAnswer to match asp-action helper
    [HttpPost("AcceptAnswer")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AcceptAnswerAlternative(Guid answerId, Guid questionId)
    {
        // TODO: Check if current user is the author of the question
        await _qaService.AcceptAnswerAsync(answerId);
        var q = await _qaService.GetQuestionByIdAsync(questionId);
        return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? questionId.ToString() });
    }

    [HttpPost("vote")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Vote(Guid entityId, EntityType entityType, VoteType voteType)
    {
        var userId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.VoteAsync(entityId, entityType, userId, voteType);

        // Redirect back to referring page or details if unsure
        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        if (entityType == EntityType.Question)
        {
            var q = await _qaService.GetQuestionByIdAsync(entityId);
            return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? entityId.ToString() });
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("bookmark")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Bookmark(Guid questionId)
    {
        var userId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.BookmarkQuestionAsync(questionId, userId);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true, bookmarked = true });
        }

        var q = await _qaService.GetQuestionByIdAsync(questionId);
        return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? questionId.ToString() });
    }

    [HttpPost("unbookmark")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Unbookmark(Guid questionId)
    {
        var userId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.UnbookmarkQuestionAsync(questionId, userId);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true, bookmarked = false });
        }

        var q = await _qaService.GetQuestionByIdAsync(questionId);
        return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? questionId.ToString() });
    }

    [HttpPost("answer/{answerId:guid}/helpful")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> MarkHelpful(Guid answerId)
    {
        var userId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.MarkHelpfulAsync(answerId, userId);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true });
        }

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction(nameof(Index));
    }

    // Alternative route for MarkHelpful to match asp-action helper
    [HttpPost("MarkHelpful")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> MarkHelpfulAlternative(Guid answerId)
    {
        var userId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.MarkHelpfulAsync(answerId, userId);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true });
        }

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction(nameof(Index));
    }

    // Admin/Moderator actions
    [HttpPost("{questionId:guid}/pin")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> PinQuestion(Guid questionId)
    {
        var moderatorId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.PinQuestionAsync(questionId, moderatorId);
        var q = await _qaService.GetQuestionByIdAsync(questionId);
        return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? questionId.ToString() });
    }

    [HttpPost("{questionId:guid}/unpin")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> UnpinQuestion(Guid questionId)
    {
        var moderatorId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.UnpinQuestionAsync(questionId, moderatorId);
        var q = await _qaService.GetQuestionByIdAsync(questionId);
        return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? questionId.ToString() });
    }

    [HttpPost("{questionId:guid}/lock")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> LockQuestion(Guid questionId, string reason)
    {
        var moderatorId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.LockQuestionAsync(questionId, reason, moderatorId);
        var q = await _qaService.GetQuestionByIdAsync(questionId);
        return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? questionId.ToString() });
    }

    [HttpPost("{questionId:guid}/unlock")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> UnlockQuestion(Guid questionId)
    {
        var moderatorId = Guid.Parse(_currentUserService.UserId!);
        await _qaService.UnlockQuestionAsync(questionId, moderatorId);
        var q = await _qaService.GetQuestionByIdAsync(questionId);
        return RedirectToAction(nameof(Details), new { slug = q?.Slug ?? questionId.ToString() });
    }

    // AJAX endpoints for dynamic content
    [HttpGet("tags")]
    public async Task<IActionResult> GetTags()
    {
        var tags = await _qaService.GetPopularTagsAsync(50);
        return Json(tags);
    }

    [HttpGet("car-makes")]
    public async Task<IActionResult> GetCarMakes()
    {
        var makes = await _qaService.GetAvailableCarMakesAsync();
        return Json(makes);
    }
    
    [HttpGet("debug")]
    public async Task<IActionResult> Debug()
    {
        try
        {
            var questions = await _qaService.GetAllQuestionsAsync();
            var result = new
            {
                QuestionsCount = questions.Count(),
                Questions = questions.Take(5).Select(q => new
                {
                    q.Id,
                    q.Title,
                    q.Slug,
                    q.CreatedAt,
                    q.AuthorId,
                    DetailsUrl = Url.Action("Details", "QA", new { slug = q.Slug })
                }).ToList(),
                Success = true
            };
            
            return Json(result);
        }
        catch (Exception ex)
        {
            return Json(new { Error = ex.Message, StackTrace = ex.StackTrace, Success = false });
        }
    }
    
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Json(new { Message = "QA Controller is working!", Controller = "QA", Action = "Test" });
    }
    
    [HttpGet("fix-slugs")]
    public async Task<IActionResult> FixSlugs()
    {
        try
        {
            // Get all questions directly from the database
            var questions = await _unitOfWork.QA.GetAllAsync();
            var fixedCount = 0;
            
            foreach (var question in questions)
            {
                if (string.IsNullOrEmpty(question.Slug))
                {
                    // Use reflection to set the slug since it has a private setter
                    var slugProperty = typeof(Question).GetProperty("Slug");
                    var generateSlugMethod = typeof(Question).GetMethod("GenerateSlug", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    
                    if (generateSlugMethod != null)
                    {
                        var newSlug = (string)generateSlugMethod.Invoke(null, new object[] { question.Title });
                        
                        // Update the question content to trigger slug regeneration
                        question.UpdateContent(question.Title, question.Body);
                        await _unitOfWork.QA.UpdateAsync(question);
                        fixedCount++;
                    }
                }
            }
            
            if (fixedCount > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            
            return Json(new { 
                Message = $"Fixed {fixedCount} questions with empty slugs", 
                TotalQuestions = questions.Count(),
                Success = true 
            });
        }
        catch (Exception ex)
        {
            return Json(new { Error = ex.Message, StackTrace = ex.StackTrace, Success = false });
        }
    }
}
public class CreateQuestionRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? BodyAr { get; set; }
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? CarEngine { get; set; }
    public string? Tags { get; set; }
}



