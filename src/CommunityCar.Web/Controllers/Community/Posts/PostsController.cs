using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Posts.DTOs;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Web.Controllers.Community.Post;

[Route("{culture}/posts")]
public class PostsController : Controller
{
    private readonly IPostsService _postsService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostsService postsService, ILogger<PostsController> logger)
    {
        _postsService = postsService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search = null,
        string? sortBy = "newest",
        PostType? filterType = null,
        int page = 1,
        int pageSize = 10)
    {
        try
        {
            var request = new PostsSearchRequest
            {
                SearchTerm = search,
                SortBy = sortBy,
                Type = filterType,
                Page = page,
                PageSize = pageSize
            };

            var response = await _postsService.SearchPostsAsync(request);

            // Convert ViewModels to simple models for the view
            var posts = response.Items.ToList();

            return View("~/Views/Community/Posts/Index.cshtml", posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading posts index");
            return View("~/Views/Community/Posts/Index.cshtml", new List<PostSummaryVM>());
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var postVM = await _postsService.GetPostByIdAsync(id);
            if (postVM == null)
            {
                return NotFound();
            }

            // Convert ViewModel to simple model for the view
            var post = postVM;

            return View("~/Views/Community/Posts/Details.cshtml", post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading post details for {PostId}", id);
            return NotFound();
        }
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> DetailsBySlug(string slug)
    {
        try
        {
            var postVM = await _postsService.GetPostBySlugAsync(slug);
            if (postVM == null)
            {
                return NotFound();
            }

            // Convert ViewModel to simple model for the view
            var post = postVM;

            return View("~/Views/Community/Posts/Details.cshtml", post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading post details for slug {Slug}", slug);
            return NotFound();
        }
    }

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create()
    {
        return View("~/Views/Community/Posts/Create.cshtml");
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(string title, string content, PostType type)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
        {
            TempData["ToasterType"] = "validation";
            TempData["ToasterTitle"] = "Validation Error";
            TempData["ToasterMessage"] = "Title and Content are required.";
            return View("~/Views/Community/Posts/Create.cshtml");
        }

        try
        {
            var request = new CreatePostRequest
            {
                Title = title,
                Content = content,
                Type = type
            };

            var postVM = await _postsService.CreatePostAsync(request);
            _logger.LogInformation("Post created: {PostTitle} with ID {PostId}", title, postVM.Id);

            TempData["ToasterType"] = "success";
            TempData["ToasterTitle"] = "Success";
            TempData["ToasterMessage"] = $"Post '{title}' has been created successfully!";

            return RedirectToAction(nameof(Details), new { id = postVM.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post: {PostTitle}", title);
            TempData["ToasterType"] = "error";
            TempData["ToasterTitle"] = "Error";
            TempData["ToasterMessage"] = "An error occurred while creating the post. Please try again.";
            return View("~/Views/Community/Posts/Create.cshtml");
        }
    }

    [HttpPost("{id:guid}/like")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Like(Guid id)
    {
        try
        {
            // TODO: Implement like functionality in service
            _logger.LogInformation("Post {PostId} liked", id);
            TempData["ToasterType"] = "success";
            TempData["ToasterTitle"] = "Success";
            TempData["ToasterMessage"] = "Post liked successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error liking post {PostId}", id);
            TempData["ToasterType"] = "error";
            TempData["ToasterTitle"] = "Error";
            TempData["ToasterMessage"] = "An error occurred while liking the post.";
        }

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("{id:guid}/comment")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Comment(Guid id, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["ToasterType"] = "validation";
            TempData["ToasterTitle"] = "Validation Error";
            TempData["ToasterMessage"] = "Comment content is required.";
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            // TODO: Implement comment functionality in service
            _logger.LogInformation("Comment added to post {PostId}", id);
            TempData["ToasterType"] = "success";
            TempData["ToasterTitle"] = "Success";
            TempData["ToasterMessage"] = "Comment added successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to post {PostId}", id);
            TempData["ToasterType"] = "error";
            TempData["ToasterTitle"] = "Error";
            TempData["ToasterMessage"] = "An error occurred while adding the comment.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("{id:guid}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var postVM = await _postsService.GetPostByIdAsync(id);
            if (postVM == null)
            {
                return NotFound();
            }

            // Convert ViewModel to simple model for the view
            var post = postVM;

            return View("~/Views/Community/Posts/Edit.cshtml", post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading post edit for {PostId}", id);
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/edit")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(Guid id, string title, string content, PostType type, 
        string? category = null, string? tags = null, bool isPinned = false, bool allowComments = true)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
        {
            TempData["ToasterType"] = "validation";
            TempData["ToasterTitle"] = "Validation Error";
            TempData["ToasterMessage"] = "Title and Content are required.";
            return await Edit(id);
        }

        try
        {
            var request = new UpdatePostRequest
            {
                Id = id,
                Title = title,
                Content = content,
                Type = type,
                Category = category,
                Tags = !string.IsNullOrWhiteSpace(tags) ? 
                    tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrEmpty(t))
                        .ToList() : new List<string>(),
                IsPinned = isPinned,
                AllowComments = allowComments
            };

            var updatedPost = await _postsService.UpdatePostAsync(id, request);
            if (updatedPost != null)
            {
                _logger.LogInformation("Post updated: {PostTitle} with ID {PostId}", title, id);
                TempData["ToasterType"] = "success";
                TempData["ToasterTitle"] = "Success";
                TempData["ToasterMessage"] = $"Post '{title}' has been updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            else
            {
                TempData["ToasterType"] = "error";
                TempData["ToasterTitle"] = "Error";
                TempData["ToasterMessage"] = "Failed to update the post. Please try again.";
                return await Edit(id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post: {PostId}", id);
            TempData["ToasterType"] = "error";
            TempData["ToasterTitle"] = "Error";
            TempData["ToasterMessage"] = "An error occurred while updating the post. Please try again.";
            return await Edit(id);
        }
    }
}



