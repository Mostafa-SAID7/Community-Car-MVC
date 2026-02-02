using CommunityCar.Application.Common.Interfaces.Services;
using CommunityCar.Application.Features.Dashboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Controllers.Dashboard;

[Authorize(Roles = "Admin,SuperAdmin,ContentAdmin,DatabaseAdmin")]
public class ErrorsController : Controller
{
    private readonly IErrorService _errorService;
    private readonly ILogger<ErrorsController> _logger;

    public ErrorsController(IErrorService errorService, ILogger<ErrorsController> logger)
    {
        _errorService = errorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, string? category = null, string? severity = null, bool? isResolved = null)
    {
        try
        {
            var (errors, pagination) = await _errorService.GetErrorsAsync(page, 50, category, severity, isResolved);
            var stats = await _errorService.GetErrorStatsAsync();
            
            ViewBag.CurrentPage = page;
            ViewBag.Category = category;
            ViewBag.Severity = severity;
            ViewBag.IsResolved = isResolved;
            ViewBag.Stats = stats;
            ViewBag.Pagination = pagination;
            
            return View("~/Views/Dashboard/Errors/Index.cshtml", errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load errors dashboard");
            TempData["ErrorMessage"] = "Failed to load errors. Please try again.";
            return View("~/Views/Dashboard/Errors/Index.cshtml", new List<CommunityCar.Domain.Entities.Shared.ErrorLog>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(string errorId)
    {
        try
        {
            var error = await _errorService.GetErrorAsync(errorId);
            if (error == null)
            {
                TempData["ErrorMessage"] = "Error not found.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load error details for {ErrorId}", errorId);
            TempData["ErrorMessage"] = "Failed to load error details.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Resolve(string errorId, string resolution)
    {
        try
        {
            var userId = User.FindFirst("id")?.Value ?? User.Identity?.Name ?? "Unknown";
            var success = await _errorService.ResolveErrorAsync(errorId, userId, resolution);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Error resolved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to resolve error.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve error {ErrorId}", errorId);
            TempData["ErrorMessage"] = "Failed to resolve error.";
        }
        
        return RedirectToAction(nameof(Details), new { errorId });
    }

    [HttpGet]
    public async Task<IActionResult> Stats(DateTime? startDate = null, DateTime? endDate = null, string? category = null)
    {
        try
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;
            
            var stats = await _errorService.GetErrorStatsRangeAsync(start, end, category);
            
            ViewBag.StartDate = start;
            ViewBag.EndDate = end;
            ViewBag.Category = category;
            
            return View("~/Views/Dashboard/Errors/Stats.cshtml", stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load error stats");
            TempData["ErrorMessage"] = "Failed to load error statistics.";
            return View("~/Views/Dashboard/Errors/Stats.cshtml", new List<ErrorStatsVM>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Boundaries(int page = 1, string? boundaryName = null, bool? isRecovered = null)
    {
        try
        {
            var (boundaries, pagination) = await _errorService.GetBoundaryErrorsAsync(page, 50, boundaryName, isRecovered);
            
            ViewBag.BoundaryName = boundaryName;
            ViewBag.IsRecovered = isRecovered;
            ViewBag.CurrentPage = page;
            ViewBag.Pagination = pagination;
            
            return View("~/Views/Dashboard/Errors/Boundaries.cshtml", boundaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load boundary errors");
            TempData["ErrorMessage"] = "Failed to load boundary errors.";
            return View("~/Views/Dashboard/Errors/Boundaries.cshtml", new List<ErrorBoundaryVM>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> RecoverBoundary(string boundaryId, string recoveryAction)
    {
        try
        {
            var success = await _errorService.RecoverBoundaryAsync(boundaryId, recoveryAction);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Boundary recovered successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to recover boundary.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to recover boundary {BoundaryId}", boundaryId);
            TempData["ErrorMessage"] = "Failed to recover boundary.";
        }
        
        return RedirectToAction(nameof(Boundaries));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStats()
    {
        try
        {
            await _errorService.UpdateErrorStatsAsync();
            TempData["SuccessMessage"] = "Error statistics updated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update error stats");
            TempData["ErrorMessage"] = "Failed to update error statistics.";
        }
        
        return RedirectToAction(nameof(Stats));
    }

    [HttpPost]
    public async Task<IActionResult> Cleanup(int daysToKeep = 90)
    {
        try
        {
            await _errorService.CleanupOldErrorsAsync(daysToKeep);
            TempData["SuccessMessage"] = $"Cleaned up errors older than {daysToKeep} days.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old errors");
            TempData["ErrorMessage"] = "Failed to cleanup old errors.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetErrorSummary()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var todayStats = await _errorService.GetErrorStatsAsync(today);
            var (totalErrors, _) = await _errorService.GetErrorsAsync(1, 1000);
            
            var summary = new
            {
                TotalErrors = totalErrors.Sum(e => e.OccurrenceCount),
                TodayErrors = todayStats.TotalErrors,
                CriticalErrors = totalErrors.Count(e => e.Severity == "Critical" && !e.IsResolved),
                UnresolvedErrors = totalErrors.Count(e => !e.IsResolved),
                ResolutionRate = totalErrors.Any() ? (double)totalErrors.Count(e => e.IsResolved) / totalErrors.Count() * 100 : 0,
                MostCommonError = todayStats.MostCommonError,
                MostCommonErrorCount = todayStats.MostCommonErrorCount
            };
            
            return Json(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get error summary");
            return Json(new { error = "Failed to load error summary" });
        }
    }
}