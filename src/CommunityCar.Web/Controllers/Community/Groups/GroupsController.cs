using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Web.Controllers.Community.Groups;

[Route("groups")]
public class GroupsController : Controller
{
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(ILogger<GroupsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search = null,
        string? sortBy = "newest",
        string? filterPrivacy = null,
        int page = 1,
        int pageSize = 12)
    {
        // Mock data for demonstration - replace with actual service call when available
        var allGroups = new List<Group>();
        
        var group1 = new Group("Classic Mustang Enthusiasts", "Dedicated to Ford Mustang lovers from 1964-1973", GroupPrivacy.Public, Guid.Empty, "Ford", null, false, "Detroit, MI");
        for (int i = 0; i < 487; i++) group1.IncrementMemberCount();
        for (int i = 0; i < 156; i++) group1.IncrementPostCount();
        group1.MarkAsOfficial();
        group1.AddTag("mustang"); group1.AddTag("ford"); group1.AddTag("muscle-car");
        allGroups.Add(group1);

        var group2 = new Group("Vintage Car Restoration", "Share tips, progress, and ask questions about restoring classic cars", GroupPrivacy.Public, Guid.Empty, "General", "Be respectful. Share knowledge. No spam.");
        for (int i = 0; i < 1243; i++) group2.IncrementMemberCount();
        for (int i = 0; i < 892; i++) group2.IncrementPostCount();
        group2.Verify();
        group2.AddTag("restoration"); group2.AddTag("diy"); group2.AddTag("classic-cars");
        allGroups.Add(group2);

        var group3 = new Group("Electric Vehicle Conversion", "Converting classic cars to electric power", GroupPrivacy.Private, Guid.Empty, "Modification", null, true);
        for (int i = 0; i < 156; i++) group3.IncrementMemberCount();
        for (int i = 0; i < 67; i++) group3.IncrementPostCount();
        group3.AddTag("ev"); group3.AddTag("electric"); group3.AddTag("modification");
        allGroups.Add(group3);

        var group4 = new Group("British Sports Cars", "MG, Triumph, Austin-Healey and more", GroupPrivacy.Public, Guid.Empty, "British", null, false, "London, UK");
        for (int i = 0; i < 678; i++) group4.IncrementMemberCount();
        for (int i = 0; i < 234; i++) group4.IncrementPostCount();
        group4.Verify();
        group4.AddTag("british"); group4.AddTag("mg"); group4.AddTag("triumph");
        allGroups.Add(group4);

        var group5 = new Group("American Muscle", "Discussing classic American muscle cars", GroupPrivacy.Public, Guid.Empty, "American");
        for (int i = 0; i < 2156; i++) group5.IncrementMemberCount();
        for (int i = 0; i < 1523; i++) group5.IncrementPostCount();
        group5.MarkAsOfficial();
        group5.AddTag("muscle"); group5.AddTag("american"); group5.AddTag("v8");
        allGroups.Add(group5);

        var group6 = new Group("European Classics", "Mercedes, BMW, Porsche vintage models", GroupPrivacy.Public, Guid.Empty, "European");
        for (int i = 0; i < 934; i++) group6.IncrementMemberCount();
        for (int i = 0; i < 445; i++) group6.IncrementPostCount();
        group6.AddTag("european"); group6.AddTag("mercedes"); group6.AddTag("bmw");
        allGroups.Add(group6);

        var group7 = new Group("JDM Legends", "Japanese Domestic Market classics", GroupPrivacy.Private, Guid.Empty, "Japanese", null, true);
        for (int i = 0; i < 567; i++) group7.IncrementMemberCount();
        for (int i = 0; i < 289; i++) group7.IncrementPostCount();
        group7.AddTag("jdm"); group7.AddTag("japan"); group7.AddTag("skyline");
        allGroups.Add(group7);

        var group8 = new Group("Corvette Club", "All generations of America's sports car", GroupPrivacy.Public, Guid.Empty, "Chevrolet", "Corvette owners and enthusiasts only. Family-friendly discussions.", false, "Bowling Green, KY");
        for (int i = 0; i < 1834; i++) group8.IncrementMemberCount();
        for (int i = 0; i < 967; i++) group8.IncrementPostCount();
        group8.MarkAsOfficial();
        group8.AddTag("corvette"); group8.AddTag("chevrolet"); group8.AddTag("sports-car");
        allGroups.Add(group8);

        var group9 = new Group("VW Air-Cooled", "Beetles, Buses, and Type 3s", GroupPrivacy.Public, Guid.Empty, "Volkswagen");
        for (int i = 0; i < 876; i++) group9.IncrementMemberCount();
        for (int i = 0; i < 523; i++) group9.IncrementPostCount();
        group9.Verify();
        group9.AddTag("vw"); group9.AddTag("beetle"); group9.AddTag("bus");
        allGroups.Add(group9);

        var group10 = new Group("Alfa Romeo Owners", "Italian passion and performance", GroupPrivacy.Private, Guid.Empty, "Italian", null, true);
        for (int i = 0; i < 234; i++) group10.IncrementMemberCount();
        for (int i = 0; i < 112; i++) group10.IncrementPostCount();
        group10.AddTag("alfa-romeo"); group10.AddTag("italian"); group10.AddTag("performance");
        allGroups.Add(group10);

        var group11 = new Group("Hot Rod Builders", "Custom builds and modifications", GroupPrivacy.Public, Guid.Empty, "Modification");
        for (int i = 0; i < 1456; i++) group11.IncrementMemberCount();
        for (int i = 0; i < 834; i++) group11.IncrementPostCount();
        group11.AddTag("hot-rod"); group11.AddTag("custom"); group11.AddTag("build");
        allGroups.Add(group11);

        var group12 = new Group("Motorcycle Classics", "Vintage motorcycles and restoration", GroupPrivacy.Public, Guid.Empty, "Motorcycle");
        for (int i = 0; i < 645; i++) group12.IncrementMemberCount();
        for (int i = 0; i < 378; i++) group12.IncrementPostCount();
        group12.AddTag("motorcycle"); group12.AddTag("vintage"); group12.AddTag("restoration");
        allGroups.Add(group12);

        var group13 = new Group("Truck & Van Classics", "Classic trucks and vans community", GroupPrivacy.Public, Guid.Empty, "Trucks");
        for (int i = 0; i < 534; i++) group13.IncrementMemberCount();
        for (int i = 0; i < 267; i++) group13.IncrementPostCount();
        group13.AddTag("truck"); group13.AddTag("van"); group13.AddTag("commercial");
        allGroups.Add(group13);

        var group14 = new Group("Racing Heritage", "Historic racing cars and motorsports", GroupPrivacy.Private, Guid.Empty, "Racing", "Serious discussions only. No off-topic posts.", true);
        for (int i = 0; i < 389; i++) group14.IncrementMemberCount();
        for (int i = 0; i < 189; i++) group14.IncrementPostCount();
        group14.Verify();
        group14.AddTag("racing"); group14.AddTag("motorsport"); group14.AddTag("historic");
        allGroups.Add(group14);

        var group15 = new Group("Parts Marketplace", "Buy, sell, and trade classic car parts", GroupPrivacy.Public, Guid.Empty, "Marketplace", "No scams. Verified sellers only. Be honest about condition.");
        for (int i = 0; i < 3421; i++) group15.IncrementMemberCount();
        for (int i = 0; i < 2134; i++) group15.IncrementPostCount();
        group15.Verify();
        group15.AddTag("parts"); group15.AddTag("marketplace"); group15.AddTag("buy-sell");
        allGroups.Add(group15);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            allGroups = allGroups.Where(g => 
                g.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                g.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        // Apply privacy filter
        if (!string.IsNullOrWhiteSpace(filterPrivacy) && filterPrivacy != "all")
        {
            if (Enum.TryParse<GroupPrivacy>(filterPrivacy, true, out var privacy))
            {
                allGroups = allGroups.Where(g => g.Privacy == privacy).ToList();
            }
        }

        // Apply sorting
        allGroups = sortBy?.ToLower() switch
        {
            "name" => allGroups.OrderBy(g => g.Name).ToList(),
            "oldest" => allGroups.OrderBy(g => g.CreatedAt).ToList(),
            "newest" or _ => allGroups.OrderByDescending(g => g.CreatedAt).ToList(),
        };

        // Calculate pagination
        var totalCount = allGroups.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

        var paginatedGroups = allGroups
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Pass pagination data to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalCount = totalCount;
        ViewBag.PageSize = pageSize;
        ViewBag.Search = search;
        ViewBag.SortBy = sortBy;
        ViewBag.FilterPrivacy = filterPrivacy;

        return await Task.FromResult(View("~/Views/Community/Groups/Index.cshtml", paginatedGroups));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        // Mock data for demonstration
        var group = new Group(
            "Classic Mustang Enthusiasts",
            "Dedicated to Ford Mustang lovers from 1964-1973. Share your restoration projects, ask questions, and connect with fellow enthusiasts.",
            GroupPrivacy.Public,
            Guid.Empty
        );

        if (group == null)
        {
            return NotFound();
        }

        return await Task.FromResult(View("~/Views/Community/Groups/Details.cshtml", group));
    }

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create()
    {
        return View("~/Views/Community/Groups/Create.cshtml");
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(string name, string description, GroupPrivacy privacy)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
        {
            ModelState.AddModelError("", "Name and Description are required.");
            return View("~/Views/Community/Groups/Create.cshtml");
        }

        // TODO: Get actual user ID and call service
        _logger.LogInformation("Group created: {Name}", name);

        return await Task.FromResult(RedirectToAction(nameof(Index)));
    }

    [HttpPost("{id:guid}/join")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Join(Guid id)
    {
        // TODO: Implement join functionality
        _logger.LogInformation("User joined group {GroupId}", id);

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return await Task.FromResult(RedirectToAction(nameof(Details), new { id }));
    }
}
