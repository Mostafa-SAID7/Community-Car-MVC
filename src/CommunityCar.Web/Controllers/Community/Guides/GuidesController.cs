using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Domain.Entities.Community.Guides;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Community.Guides;

[Route("guides")]
public class GuidesController : Controller
{
    private readonly ILogger<GuidesController> _logger;

    public GuidesController(ILogger<GuidesController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search = null,
        string? category = null,
        GuideDifficulty? difficulty = null,
        string? sortBy = "newest",
        int page = 1,
        int pageSize = 12)
    {
        // Mock data for demonstration
        var allGuides = new List<Guide>();

        var guide1 = new Guide(
            "Complete Engine Oil Change Guide",
            "Learn how to properly change your engine oil...",
            Guid.Empty,
            "Step-by-step instructions for changing engine oil in classic cars",
            "Maintenance",
            GuideDifficulty.Beginner,
            45
        );
        guide1.Publish();
        guide1.Feature();
        for (int i = 0; i < 3456; i++) guide1.IncrementViewCount();
        for (int i = 0; i < 234; i++) guide1.IncrementBookmarkCount();
        guide1.AddRating(5); guide1.AddRating(4.5); guide1.AddRating(5); guide1.AddRating(4);
        guide1.AddTag("oil-change"); guide1.AddTag("maintenance"); guide1.AddTag("beginner");
        guide1.AddRequiredTool("Oil filter wrench"); guide1.AddRequiredTool("Oil drain pan"); guide1.AddRequiredTool("Funnel");
        allGuides.Add(guide1);

        var guide2 = new Guide(
            "Brake System Overhaul - Complete Guide",
            "Master class on brake system maintenance and replacement...",
            Guid.Empty,
            "Comprehensive guide to brake system repair and maintenance",
            "Brakes",
            GuideDifficulty.Intermediate,
            180
        );
        guide2.Publish();
        guide2.Verify();
        for (int i = 0; i < 2134; i++) guide2.IncrementViewCount();
        for (int i = 0; i < 156; i++) guide2.IncrementBookmarkCount();
        guide2.AddRating(4.5); guide2.AddRating(5); guide2.AddRating(4.5);
        guide2.AddTag("brakes"); guide2.AddTag("safety"); guide2.AddTag("intermediate");
        guide2.AddPrerequisite("Basic automotive knowledge");
        guide2.AddRequiredTool("Brake bleeder kit"); guide2.AddRequiredTool("Torque wrench");
        allGuides.Add(guide2);

        var guide3 = new Guide(
            "Carburetor Tuning for Vintage Cars",
            "Advanced tuning guide for classic carburetors...",
            Guid.Empty,
            "Expert-level carburetor adjustment and tuning techniques",
            "Engine",
            GuideDifficulty.Advanced,
            120
        );
        guide3.Publish();
        guide3.Verify();
        for (int i = 0; i < 1567; i++) guide3.IncrementViewCount();
        for (int i = 0; i < 89; i++) guide3.IncrementBookmarkCount();
        guide3.AddRating(5); guide3.AddRating(4.5); guide3.AddRating(5);
        guide3.AddTag("carburetor"); guide3.AddTag("tuning"); guide3.AddTag("advanced");
        guide3.AddPrerequisite("Understanding of carburetor basics");
        guide3.AddRequiredTool("Carburetor synchronizer"); guide3.AddRequiredTool("AFR gauge");
        allGuides.Add(guide3);

        var guide4 = new Guide(
            "Paint Restoration Techniques",
            "Bring back the shine to your classic car's paint...",
            Guid.Empty,
            "Professional paint correction and restoration methods",
            "Bodywork",
            GuideDifficulty.Intermediate,
            240
        );
        guide4.Publish();
        guide4.Feature();
        for (int i = 0; i < 2876; i++) guide4.IncrementViewCount();
        for (int i = 0; i < 198; i++) guide4.IncrementBookmarkCount();
        guide4.AddRating(5); guide4.AddRating(5); guide4.AddRating(4.5); guide4.AddRating(5);
        guide4.AddTag("paint"); guide4.AddTag("restoration"); guide4.AddTag("detailing");
        guide4.AddRequiredTool("Dual-action polisher"); guide4.AddRequiredTool("Compound and polish");
        allGuides.Add(guide4);

        var guide5 = new Guide(
            "Electrical System Diagnostics",
            "Troubleshoot and repair electrical issues in classic cars...",
            Guid.Empty,
            "Systematic approach to diagnosing electrical problems",
            "Electrical",
            GuideDifficulty.Advanced,
            90
        );
        guide5.Publish();
        guide5.Verify();
        for (int i = 0; i < 1834; i++) guide5.IncrementViewCount();
        for (int i = 0; i < 112; i++) guide5.IncrementBookmarkCount();
        guide5.AddRating(4.5); guide5.AddRating(4); guide5.AddRating(5);
        guide5.AddTag("electrical"); guide5.AddTag("diagnostics"); guide5.AddTag("troubleshooting");
        guide5.AddPrerequisite("Basic electrical knowledge");
        guide5.AddRequiredTool("Multimeter"); guide5.AddRequiredTool("Wiring diagram");
        allGuides.Add(guide5);

        var guide6 = new Guide(
            "Transmission Fluid Service",
            "Keep your transmission running smooth with proper fluid maintenance...",
            Guid.Empty,
            "Complete guide to transmission fluid checking and changing",
            "Transmission",
            GuideDifficulty.Beginner,
            60
        );
        guide6.Publish();
        for (int i = 0; i < 1456; i++) guide6.IncrementViewCount();
        for (int i = 0; i < 78; i++) guide6.IncrementBookmarkCount();
        guide6.AddRating(4); guide6.AddRating(4.5); guide6.AddRating(4.5);
        guide6.AddTag("transmission"); guide6.AddTag("fluid"); guide6.AddTag("maintenance");
        guide6.AddRequiredTool("Transmission fluid pump"); guide6.AddRequiredTool("Drain pan");
        allGuides.Add(guide6);

        var guide7 = new Guide(
            "Suspension Upgrade Guide",
            "Modernize your classic's handling with suspension upgrades...",
            Guid.Empty,
            "Comprehensive guide to suspension modification and improvement",
            "Suspension",
            GuideDifficulty.Expert,
            300
        );
        guide7.Publish();
        guide7.Verify();
        for (int i = 0; i < 987; i++) guide7.IncrementViewCount();
        for (int i = 0; i < 67; i++) guide7.IncrementBookmarkCount();
        guide7.AddRating(5); guide7.AddRating(5);
        guide7.AddTag("suspension"); guide7.AddTag("upgrade"); guide7.AddTag("expert");
        guide7.AddPrerequisite("Advanced mechanical skills");
        guide7.AddRequiredTool("Spring compressor"); guide7.AddRequiredTool("Alignment tools");
        allGuides.Add(guide7);

        var guide8 = new Guide(
            "Interior Restoration Basics",
            "Restore your classic car's interior to showroom condition...",
            Guid.Empty,
            "Step-by-step interior cleaning, repair, and restoration",
            "Interior",
            GuideDifficulty.Beginner,
            150
        );
        guide8.Publish();
        guide8.Feature();
        for (int i = 0; i < 2345; i++) guide8.IncrementViewCount();
        for (int i = 0; i < 145; i++) guide8.IncrementBookmarkCount();
        guide8.AddRating(4.5); guide8.AddRating(5); guide8.AddRating(4.5); guide8.AddRating(5);
        guide8.AddTag("interior"); guide8.AddTag("restoration"); guide8.AddTag("upholstery");
        guide8.AddRequiredTool("Upholstery cleaner"); guide8.AddRequiredTool("Leather conditioner");
        allGuides.Add(guide8);

        var guide9 = new Guide(
            "Rust Prevention and Treatment",
            "Stop rust in its tracks and restore affected areas...",
            Guid.Empty,
            "Effective methods for rust prevention and removal",
            "Bodywork",
            GuideDifficulty.Intermediate,
            120
        );
        guide9.Publish();
        for (int i = 0; i < 1678; i++) guide9.IncrementViewCount();
        for (int i = 0; i < 94; i++) guide9.IncrementBookmarkCount();
        guide9.AddRating(4.5); guide9.AddRating(4); guide9.AddRating(4.5);
        guide9.AddTag("rust"); guide9.AddTag("prevention"); guide9.AddTag("bodywork");
        guide9.AddRequiredTool("Wire brush"); guide9.AddRequiredTool("Rust converter");
        allGuides.Add(guide9);

        var guide10 = new Guide(
            "Cooling System Maintenance",
            "Prevent overheating with proper cooling system care...",
            Guid.Empty,
            "Complete cooling system inspection and maintenance guide",
            "Engine",
            GuideDifficulty.Beginner,
            75
        );
        guide10.Publish();
        for (int i = 0; i < 1234; i++) guide10.IncrementViewCount();
        for (int i = 0; i < 56; i++) guide10.IncrementBookmarkCount();
        guide10.AddRating(4); guide10.AddRating(4.5);
        guide10.AddTag("cooling"); guide10.AddTag("radiator"); guide10.AddTag("maintenance");
        guide10.AddRequiredTool("Coolant tester"); guide10.AddRequiredTool("Funnel");
        allGuides.Add(guide10);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(search))
        {
            allGuides = allGuides.Where(g =>
                g.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (g.Summary != null && g.Summary.Contains(search, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(category) && category != "all")
        {
            allGuides = allGuides.Where(g => g.Category == category).ToList();
        }

        if (difficulty.HasValue)
        {
            allGuides = allGuides.Where(g => g.Difficulty == difficulty.Value).ToList();
        }

        // Apply sorting
        allGuides = sortBy?.ToLower() switch
        {
            "popular" => allGuides.OrderByDescending(g => g.ViewCount).ToList(),
            "rating" => allGuides.OrderByDescending(g => g.AverageRating).ToList(),
            "newest" or _ => allGuides.OrderByDescending(g => g.PublishedAt ?? g.CreatedAt).ToList(),
        };

        // Calculate pagination
        var totalCount = allGuides.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

        var paginatedGuides = allGuides
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalCount = totalCount;
        ViewBag.Search = search;
        ViewBag.Category = category;
        ViewBag.Difficulty = difficulty;
        ViewBag.SortBy = sortBy;

        return await Task.FromResult(View("~/Views/Community/Guides/Index.cshtml", paginatedGuides));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        // Mock data
        var guide = new Guide(
            "Complete Engine Oil Change Guide",
            "## Introduction\n\nChanging your engine oil is one of the most important maintenance tasks...\n\n## Tools Required\n\n- Oil filter wrench\n- Oil drain pan\n- Funnel\n\n## Steps\n\n1. **Warm up the engine** - Run the car for 5 minutes\n2. **Locate the drain plug** - Found under the oil pan\n3. **Drain the oil** - Remove plug and let oil drain completely...",
            Guid.Empty,
            "Step-by-step instructions for changing engine oil in classic cars",
            "Maintenance",
            GuideDifficulty.Beginner,
            45
        );
        guide.Publish();
        
        if (guide == null)
        {
            return NotFound();
        }

        return await Task.FromResult(View("~/Views/Community/Guides/Details.cshtml", guide));
    }

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create()
    {
        return View("~/Views/Community/Guides/Create.cshtml");
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(string title, string content, string? summary, string? category, GuideDifficulty difficulty, int estimatedMinutes)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
        {
            ModelState.AddModelError("", "Title and Content are required.");
            return View("~/Views/Community/Guides/Create.cshtml");
        }

        // TODO: Get actual user ID and call service
        _logger.LogInformation("Guide created: {Title}", title);

        return await Task.FromResult(RedirectToAction(nameof(Index)));
    }

    [HttpPost("{id:guid}/bookmark")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Bookmark(Guid id)
    {
        // TODO: Implement bookmark functionality
        _logger.LogInformation("Guide {GuideId} bookmarked", id);

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return await Task.FromResult(RedirectToAction(nameof(Details), new { id }));
    }
}
