using Microsoft.AspNetCore.Mvc;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Features.Maps.DTOs;
using CommunityCar.Domain.Enums.Community;
using Microsoft.AspNetCore.Authorization;

namespace CommunityCar.Web.Controllers.Community.Maps;

[Route("{culture}/maps")]
public class MapsController : Controller
{
    private readonly IMapsService _mapsService;
    private readonly IReviewsService _reviewsService;

    public MapsController(IMapsService mapsService, IReviewsService reviewsService)
    {
        _mapsService = mapsService;
        _reviewsService = reviewsService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _mapsService.GetMapsStatsAsync();
        return View("~/Views/Community/Maps/Index.cshtml", stats);
    }

    [HttpGet("poi")]
    public async Task<IActionResult> POIList([FromQuery] MapsSearchRequest request)
    {
        var result = await _mapsService.SearchPointsOfInterestAsync(request);
        
        // Pass search parameters to view for form persistence
        ViewBag.SearchTerm = request.SearchTerm;
        ViewBag.Type = request.Type?.ToString();
        ViewBag.Category = request.Category?.ToString();
        ViewBag.IsVerified = request.IsVerified?.ToString().ToLower();
        ViewBag.IsOpen24Hours = request.IsOpen24Hours?.ToString().ToLower();
        
        return View("~/Views/Community/Maps/POIList.cshtml", result);
    }

    [HttpGet("routes")]
    public async Task<IActionResult> RouteList([FromQuery] MapsRouteSearchRequest request)
    {
        var result = await _mapsService.SearchRoutesAsync(request);
        
        // Pass search parameters to view for form persistence
        ViewBag.SearchTerm = request.SearchTerm;
        ViewBag.Type = request.Type?.ToString();
        ViewBag.Difficulty = request.Difficulty?.ToString();
        ViewBag.IsScenic = request.IsScenic?.ToString().ToLower();
        ViewBag.IsOffRoad = request.IsOffRoad?.ToString().ToLower();
        ViewBag.HasTolls = request.HasTolls == false ? "true" : "false"; // Inverted for "No Tolls" checkbox
        
        return View("~/Views/Community/Maps/RouteList.cshtml", result);
    }

    [HttpGet("poi/{id}")]
    public async Task<IActionResult> POIDetails(Guid id)
    {
        var poi = await _mapsService.GetPointOfInterestByIdAsync(id);
        if (poi == null)
            return NotFound();

        // Fetch reviews
        var reviews = await _reviewsService.GetReviewsByTargetAsync(id, "POI");
        poi.Reviews = reviews.ToList();

        return View("~/Views/Community/Maps/POIDetails.cshtml", poi);
    }

    [HttpGet("routes/{id}")]
    public async Task<IActionResult> RouteDetails(Guid id)
    {
        var route = await _mapsService.GetRouteByIdAsync(id);
        if (route == null)
            return NotFound();

        // Fetch reviews
        var reviews = await _reviewsService.GetReviewsByTargetAsync(id, "Route");
        route.Reviews = reviews.ToList();

        return View("~/Views/Community/Maps/RouteDetails.cshtml", route);
    }

    [HttpGet("poi/{id}/checkins")]
    public async Task<IActionResult> POICheckIns(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var poi = await _mapsService.GetPointOfInterestByIdAsync(id);
        if (poi == null)
            return NotFound();

        var checkIns = await _mapsService.GetCheckInsAsync(id, page, pageSize);
        
        ViewBag.POIName = poi.Name;
        ViewBag.POIId = id.ToString();
        
        return View("~/Views/Community/Maps/CheckIns.cshtml", checkIns);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] MapsSearchRequest request)
    {
        var result = await _mapsService.SearchPointsOfInterestAsync(request);
        return Json(result);
    }

    [HttpGet("routes/search")]
    public async Task<IActionResult> SearchRoutes([FromQuery] MapsRouteSearchRequest request)
    {
        var result = await _mapsService.SearchRoutesAsync(request);
        return Json(result);
    }

    [HttpGet("poi/{id}/json")]
    public async Task<IActionResult> GetPointOfInterestJson(Guid id)
    {
        var poi = await _mapsService.GetPointOfInterestByIdAsync(id);
        if (poi == null)
            return NotFound();

        return Json(poi);
    }

    [HttpGet("routes/{id}/json")]
    public async Task<IActionResult> GetRouteJson(Guid id)
    {
        var route = await _mapsService.GetRouteByIdAsync(id);
        if (route == null)
            return NotFound();

        return Json(route);
    }

    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearbyPOIs(
        [FromQuery] double latitude, 
        [FromQuery] double longitude, 
        [FromQuery] double radius = 10,
        [FromQuery] POIType? type = null)
    {
        var pois = await _mapsService.GetNearbyPointsOfInterestAsync(latitude, longitude, radius, type);
        return Json(pois);
    }

    [HttpGet("routes/nearby")]
    public async Task<IActionResult> GetNearbyRoutes(
        [FromQuery] double latitude, 
        [FromQuery] double longitude, 
        [FromQuery] double radius = 50)
    {
        var routes = await _mapsService.GetNearbyRoutesAsync(latitude, longitude, radius);
        return Json(routes);
    }

    [HttpPost("poi")]
    [Authorize]
    public async Task<IActionResult> CreatePointOfInterest([FromBody] CreatePointOfInterestRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var poi = await _mapsService.CreatePointOfInterestAsync(request);
            return Json(poi);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("poi/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePointOfInterest(Guid id, [FromBody] UpdatePointOfInterestRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var poi = await _mapsService.UpdatePointOfInterestAsync(id, request);
            return Json(poi);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("poi/{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePointOfInterest(Guid id)
    {
        try
        {
            var success = await _mapsService.DeletePointOfInterestAsync(id);
            if (!success)
                return NotFound();

            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("poi/{id}/checkin")]
    [Authorize]
    public async Task<IActionResult> CheckIn(Guid id, [FromBody] CreateCheckInRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var checkIn = await _mapsService.CheckInAsync(id, request);
            return Json(checkIn);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("poi/{id}/checkins/api")]
    public async Task<IActionResult> GetCheckIns(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var checkIns = await _mapsService.GetCheckInsAsync(id, page, pageSize);
        return Json(checkIns);
    }

    [HttpPost("routes")]
    [Authorize]
    public async Task<IActionResult> CreateRoute([FromBody] CreateRouteRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var route = await _mapsService.CreateRouteAsync(request);
            return Json(route);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("routes/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] UpdateRouteRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var route = await _mapsService.UpdateRouteAsync(id, request);
            return Json(route);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("routes/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteRoute(Guid id)
    {
        try
        {
            var success = await _mapsService.DeleteRouteAsync(id);
            if (!success)
                return NotFound();

            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("routes/{id}/complete")]
    [Authorize]
    public async Task<IActionResult> CompleteRoute(Guid id)
    {
        var success = await _mapsService.CompleteRouteAsync(id);
        if (!success)
            return NotFound();

        return Ok();
    }

    [HttpPost("poi/{id}/verify")]
    [Authorize] // In a real app, this would require admin role
    public async Task<IActionResult> VerifyPointOfInterest(Guid id)
    {
        // This should be restricted to admins/moderators
        var currentUserId = Guid.NewGuid(); // Get from current user service
        var success = await _mapsService.VerifyPointOfInterestAsync(id, currentUserId);
        if (!success)
            return NotFound();

        return Ok();
    }

    [HttpPost("poi/{id}/report")]
    [Authorize]
    public async Task<IActionResult> ReportPointOfInterest(Guid id)
    {
        var success = await _mapsService.ReportPointOfInterestAsync(id);
        if (!success)
            return NotFound();

        return Ok();
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _mapsService.GetMapsStatsAsync();
        return Json(stats);
    }
}



