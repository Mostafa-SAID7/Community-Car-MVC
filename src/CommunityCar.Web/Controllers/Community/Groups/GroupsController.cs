using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Groups.DTOs;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Web.Controllers.Community.Groups;

[Route("{culture}/groups")]
public class GroupsController : Controller
{
    private readonly IGroupsService _groupsService;
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(IGroupsService groupsService, ILogger<GroupsController> logger)
    {
        _groupsService = groupsService;
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
        try
        {
            var request = new GroupsSearchRequest
            {
                SearchTerm = search,
                SortBy = sortBy,
                Privacy = !string.IsNullOrWhiteSpace(filterPrivacy) && filterPrivacy != "all" && 
                         Enum.TryParse<GroupPrivacy>(filterPrivacy, true, out var privacy) ? privacy : null,
                Page = page,
                PageSize = pageSize
            };

            var response = await _groupsService.SearchGroupsAsync(request);

            // Pass pagination data to view
            ViewBag.CurrentPage = response.Page;
            ViewBag.TotalPages = (int)Math.Ceiling(response.TotalCount / (double)response.PageSize);
            ViewBag.TotalCount = response.TotalCount;
            ViewBag.PageSize = response.PageSize;
            ViewBag.Search = search;
            ViewBag.SortBy = sortBy;
            ViewBag.FilterPrivacy = filterPrivacy;

            // Convert ViewModels to a simple model for the view
            var groups = response.Items.ToList();

            return View("~/Views/Community/Groups/Index.cshtml", groups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading groups index");
            return View("~/Views/Community/Groups/Index.cshtml", new List<CommunityCar.Domain.Entities.Community.Groups.Group>());
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var groupVM = await _groupsService.GetGroupByIdAsync(id);
            if (groupVM == null)
            {
                return NotFound();
            }

            // Convert ViewModel to a simple model for the view
            var group = groupVM;

            return View("~/Views/Community/Groups/Details.cshtml", group);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading group details for {GroupId}", id);
            return NotFound();
        }
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> DetailsBySlug(string slug)
    {
        try
        {
            var groupVM = await _groupsService.GetGroupBySlugAsync(slug);
            if (groupVM == null)
            {
                return NotFound();
            }

            // Convert ViewModel to a simple model for the view
            var group = groupVM;

            return View("~/Views/Community/Groups/Details.cshtml", group);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading group details for slug {Slug}", slug);
            return NotFound();
        }
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
    public async Task<IActionResult> Create(string name, string description, GroupPrivacy privacy, 
        string? category = null, string? location = null, string? rules = null)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
        {
            TempData["ToasterType"] = "validation";
            TempData["ToasterTitle"] = "Validation Error";
            TempData["ToasterMessage"] = "Name and Description are required.";
            return View("~/Views/Community/Groups/Create.cshtml");
        }

        try
        {
            var request = new CreateGroupRequest
            {
                Name = name,
                Description = description,
                Privacy = privacy,
                Category = category,
                Location = location,
                Rules = rules,
                RequiresApproval = privacy == GroupPrivacy.Private
            };

            var groupVM = await _groupsService.CreateGroupAsync(request);
            _logger.LogInformation("Group created: {GroupName} with ID {GroupId}", name, groupVM.Id);

            TempData["ToasterType"] = "success";
            TempData["ToasterTitle"] = "Success";
            TempData["ToasterMessage"] = $"Group '{name}' has been created successfully!";

            return RedirectToAction(nameof(Details), new { id = groupVM.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group: {GroupName}", name);
            TempData["ToasterType"] = "error";
            TempData["ToasterTitle"] = "Error";
            TempData["ToasterMessage"] = "An error occurred while creating the group. Please try again.";
            return View("~/Views/Community/Groups/Create.cshtml");
        }
    }

    [HttpPost("{id:guid}/join")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Join(Guid id)
    {
        try
        {
            var success = await _groupsService.JoinGroupAsync(id);
            if (success)
            {
                _logger.LogInformation("User joined group {GroupId}", id);
                TempData["ToasterType"] = "success";
                TempData["ToasterTitle"] = "Success";
                TempData["ToasterMessage"] = "You have successfully joined the group!";
            }
            else
            {
                _logger.LogWarning("Failed to join group {GroupId}", id);
                TempData["ToasterType"] = "warning";
                TempData["ToasterTitle"] = "Warning";
                TempData["ToasterMessage"] = "Unable to join the group. It may require approval or be full.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining group {GroupId}", id);
            TempData["ToasterType"] = "error";
            TempData["ToasterTitle"] = "Error";
            TempData["ToasterMessage"] = "An error occurred while joining the group. Please try again.";
        }

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("{id:guid}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var groupVM = await _groupsService.GetGroupByIdAsync(id);
            if (groupVM == null)
            {
                return NotFound();
            }

            // Convert ViewModel to simple model for the view
            var group = groupVM;

            return View("~/Views/Community/Groups/Edit.cshtml", group);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading group edit for {GroupId}", id);
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/edit")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(Guid id, string name, string description, GroupPrivacy privacy, 
        string? category = null, string? location = null, string? rules = null, bool requiresApproval = false)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
        {
            TempData["ToasterType"] = "validation";
            TempData["ToasterTitle"] = "Validation Error";
            TempData["ToasterMessage"] = "Name and Description are required.";
            return await Edit(id);
        }

        try
        {
            var request = new UpdateGroupRequest
            {
                Id = id,
                Name = name,
                Description = description,
                Privacy = privacy,
                Category = category,
                Location = location,
                Rules = rules,
                RequiresApproval = requiresApproval
            };

            var updatedGroup = await _groupsService.UpdateGroupAsync(id, request);
            if (updatedGroup != null)
            {
                _logger.LogInformation("Group updated: {GroupName} with ID {GroupId}", name, id);
                TempData["ToasterType"] = "success";
                TempData["ToasterTitle"] = "Success";
                TempData["ToasterMessage"] = $"Group '{name}' has been updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            else
            {
                TempData["ToasterType"] = "error";
                TempData["ToasterTitle"] = "Error";
                TempData["ToasterMessage"] = "Failed to update the group. Please try again.";
                return await Edit(id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group: {GroupId}", id);
            TempData["ToasterType"] = "error";
            TempData["ToasterTitle"] = "Error";
            TempData["ToasterMessage"] = "An error occurred while updating the group. Please try again.";
            return await Edit(id);
        }
    }
}



