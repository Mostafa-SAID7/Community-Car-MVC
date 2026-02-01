using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Community.Events.DTOs;
using CommunityCar.Application.Features.Community.Events.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CommunityCar.Web.Controllers.Community.Events;

[Route("{culture}/events")]
public class EventsController : Controller
{
    private readonly IEventsService _eventsService;

    public EventsController(IEventsService eventsService)
    {
        _eventsService = eventsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] EventsSearchRequest request)
    {
        request.Page = Math.Max(1, request.Page);
        request.PageSize = Math.Min(50, Math.Max(1, request.PageSize == 0 ? 12 : request.PageSize));

        var response = await _eventsService.SearchEventsAsync(request);
        var stats = await _eventsService.GetEventsStatsAsync();

        var viewModel = new EventsIndexVM
        {
            SearchRequest = request,
            SearchResponse = response,
            Stats = stats
        };

        return View("~/Views/Community/Events/Index.cshtml", viewModel);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var eventItem = await _eventsService.GetEventByIdAsync(id);
        if (eventItem == null)
        {
            return NotFound();
        }

        return View("~/Views/Community/Events/Details.cshtml", eventItem);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> DetailsBySlug(string slug)
    {
        var eventItem = await _eventsService.GetEventBySlugAsync(slug);
        if (eventItem == null)
        {
            return NotFound();
        }

        return View("~/Views/Community/Events/Details.cshtml", eventItem);
    }

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create()
    {
        var viewModel = new CreateEventRequest
        {
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(2),
            IsPublic = true,
            RequiresApproval = false
        };

        return View("~/Views/Community/Events/Create.cshtml", viewModel);
    }

    [HttpPost("create")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Community/Events/Create.cshtml", request);
        }

        try
        {
            var eventItem = await _eventsService.CreateEventAsync(request);
            TempData["SuccessMessage"] = "Event created successfully!";
            return RedirectToAction(nameof(Details), new { id = eventItem.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("~/Views/Community/Events/Create.cshtml", request);
        }
    }

    [HttpGet("{id:guid}/edit")]
    [Authorize]
    public async Task<IActionResult> Edit(Guid id)
    {
        var eventItem = await _eventsService.GetEventByIdAsync(id);
        if (eventItem == null)
        {
            return NotFound();
        }

        var request = new UpdateEventRequest
        {
            Title = eventItem.Title,
            Description = eventItem.Description,
            StartTime = eventItem.StartTime,
            EndTime = eventItem.EndTime,
            Location = eventItem.Location,
            LocationDetails = eventItem.LocationDetails,
            Latitude = eventItem.Latitude,
            Longitude = eventItem.Longitude,
            MaxAttendees = eventItem.MaxAttendees,
            RequiresApproval = eventItem.RequiresApproval,
            TicketPrice = eventItem.TicketPrice,
            TicketInfo = eventItem.TicketInfo,
            IsPublic = eventItem.IsPublic,
            ExternalUrl = eventItem.ExternalUrl,
            ContactInfo = eventItem.ContactInfo
        };

        return View("~/Views/Community/Events/Edit.cshtml", request);
    }

    [HttpPost("{id:guid}/edit")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateEventRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Community/Events/Edit.cshtml", request);
        }

        try
        {
            var eventItem = await _eventsService.UpdateEventAsync(id, request);
            TempData["SuccessMessage"] = "Event updated successfully!";
            return RedirectToAction(nameof(Details), new { id = eventItem.Id });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("~/Views/Community/Events/Edit.cshtml", request);
        }
    }

    [HttpPost("{id:guid}/delete")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _eventsService.DeleteEventAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Event not found.";
            }
        }
        catch (UnauthorizedAccessException)
        {
            TempData["ErrorMessage"] = "You don't have permission to delete this event.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("my-events")]
    [Authorize]
    public async Task<IActionResult> MyEvents()
    {
        var currentUserIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdString) || !Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            return Unauthorized();
        }

        var events = await _eventsService.GetUserEventsAsync(currentUserId);
        return View("~/Views/Community/Events/MyEvents.cshtml", events);
    }

    [HttpPost("{id:guid}/join")]
    [Authorize]
    public async Task<IActionResult> Join(Guid id)
    {
        try
        {
            var success = await _eventsService.JoinEventAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Successfully joined the event!" });
            }
            else
            {
                return Json(new { success = false, message = "Event not found." });
            }
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "An error occurred while joining the event." });
        }
    }

    [HttpPost("{id:guid}/leave")]
    [Authorize]
    public async Task<IActionResult> Leave(Guid id)
    {
        try
        {
            var success = await _eventsService.LeaveEventAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Successfully left the event!" });
            }
            else
            {
                return Json(new { success = false, message = "Event not found." });
            }
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "An error occurred while leaving the event." });
        }
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> Upcoming()
    {
        var events = await _eventsService.GetUpcomingEventsAsync(20);
        return View("~/Views/Community/Events/Upcoming.cshtml", events);
    }
}



