using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Domain.Entities.Community.Posts;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Community.Posts;

[Route("posts")]
public class PostsController : Controller
{
    private readonly ILogger<PostsController> _logger;

    public PostsController(ILogger<PostsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        // Mock data for demonstration - replace with actual service call when available
        var posts = new List<Post>
        {
            new Post(
                "How to maintain your classic car during winter?",
                "Winter can be tough on classic cars. Here are some essential tips to keep your vehicle in top shape during the cold months...",
                PostType.Text,
                Guid.Empty
            ),
            new Post(
                "My 1967 Mustang Restoration Journey",
                "After 6 months of hard work, my Mustang is finally coming together. Check out the progress!",
                PostType.Image,
                Guid.Empty
            ),
            new Post(
                "Best engine oils for vintage cars",
                "https://example.com/best-oils-vintage-cars - A comprehensive guide on choosing the right engine oil",
                PostType.Link,
                Guid.Empty
            ),
            new Post(
                "What's your dream classic car?",
                "Let's discuss! What classic car would you love to own and why?",
                PostType.Poll,
                Guid.Empty
            )
        };

        return await Task.FromResult(View("~/Views/Community/Posts/Index.cshtml", posts));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        // Mock data for demonstration - replace with actual service call when available
        var post = new Post(
            "How to maintain your classic car during winter?",
            "Winter can be tough on classic cars. Here are some essential tips to keep your vehicle in top shape during the cold months.\n\n1. **Storage**: Store your car in a dry, climate-controlled garage.\n2. **Battery**: Disconnect the battery or use a trickle charger.\n3. **Fuel**: Add fuel stabilizer to prevent degradation.\n4. **Tires**: Inflate tires to the proper pressure to prevent flat spots.\n5. **Cover**: Use a breathable car cover to protect the paint.",
            PostType.Text,
            Guid.Empty
        );

        if (post == null)
        {
            return NotFound();
        }

        return await Task.FromResult(View("~/Views/Community/Posts/Details.cshtml", post));
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
            ModelState.AddModelError("", "Title and Content are required.");
            return View("~/Views/Community/Posts/Create.cshtml");
        }

        // TODO: Get actual user ID from ICurrentUserService
        var authorId = Guid.Empty;
        
        // TODO: Call service to create post
        _logger.LogInformation("Post created: {Title} by {AuthorId}", title, authorId);

        return await Task.FromResult(RedirectToAction(nameof(Index)));
    }

    [HttpPost("{id:guid}/like")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Like(Guid id)
    {
        // TODO: Implement like functionality
        _logger.LogInformation("Post {PostId} liked", id);

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return await Task.FromResult(RedirectToAction(nameof(Details), new { id }));
    }

    [HttpPost("{id:guid}/comment")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Comment(Guid id, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        // TODO: Implement comment functionality
        _logger.LogInformation("Comment added to post {PostId}", id);

        return await Task.FromResult(RedirectToAction(nameof(Details), new { id }));
    }
}
