using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Localization;
using CommunityCar.Domain.Entities.Localization;
using CommunityCar.Application.Features.Dashboard.Localization.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace CommunityCar.Web.Controllers.Dashboard.Settings;

[Route("dashboard/settings/localization")]
public class LocalizationController : Controller
{
    private readonly ILocalizationService _localizationService;

    public LocalizationController(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var cultures = await _localizationService.GetSupportedCulturesAsync();
        return View("~/Views/Dashboard/Localization/Index.cshtml", cultures);
    }

    [HttpGet("resources")]
    public async Task<IActionResult> Resources(string? culture, string? group, string? search, int page = 1, int pageSize = 20)
    {
        var (resources, totalCount) = await _localizationService.GetPaginatedResourcesAsync(culture, group, search, page, pageSize);
        
        var viewModel = new ResourcesVM
        {
            Resources = resources,
            Cultures = await _localizationService.GetSupportedCulturesAsync(),
            ResourceGroups = await _localizationService.GetResourceGroupsAsync(),
            SelectedCulture = culture,
            SelectedGroup = group,
            SearchTerm = search,
            Pagination = new CommunityCar.Application.Common.Models.PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPreviousPage = page > 1,
                HasNextPage = page < (int)Math.Ceiling(totalCount / (double)pageSize),
                StartItem = ((page - 1) * pageSize) + 1,
                EndItem = Math.Min(page * pageSize, totalCount)
            }
        };

        return View("~/Views/Dashboard/Localization/Resources.cshtml", viewModel);
    }

    [HttpGet("resources/add")]
    public async Task<IActionResult> AddResource()
    {
        ViewBag.Cultures = await _localizationService.GetSupportedCulturesAsync();
        ViewBag.ResourceGroups = await _localizationService.GetResourceGroupsAsync();
        return View("~/Views/Dashboard/Localization/EditResource.cshtml", new LocalizationResource());
    }

    [HttpGet("resources/edit/{id:guid}")]
    public async Task<IActionResult> EditResource(Guid id)
    {
        var resource = (await _localizationService.GetAllResourcesAsync()).FirstOrDefault(r => r.Id == id);
        if (resource == null) return NotFound();

        ViewBag.Cultures = await _localizationService.GetSupportedCulturesAsync();
        ViewBag.ResourceGroups = await _localizationService.GetResourceGroupsAsync();
        return View("~/Views/Dashboard/Localization/EditResource.cshtml", resource);
    }

    [HttpPost("resources/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveResource(LocalizationResource resource)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Cultures = await _localizationService.GetSupportedCulturesAsync();
            return View("~/Views/Dashboard/Localization/EditResource.cshtml", resource);
        }

        await _localizationService.SetResourceValueAsync(resource.Key, resource.Value, resource.Culture, resource.ResourceGroup);
        return RedirectToAction(nameof(Resources));
    }

    [HttpPost("resources/delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteResource(Guid id)
    {
        await _localizationService.DeleteResourceAsync(id);
        return RedirectToAction(nameof(Resources));
    }

    [HttpGet("cultures/add")]
    public IActionResult AddCulture()
    {
        return View("~/Views/Dashboard/Localization/EditCulture.cshtml", new LocalizationCulture());
    }

    [HttpPost("cultures/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCulture(LocalizationCulture culture)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Dashboard/Localization/EditCulture.cshtml", culture);
        }

        if (culture.Id == Guid.Empty)
        {
            await _localizationService.AddCultureAsync(culture);
        }
        else
        {
            await _localizationService.UpdateCultureAsync(culture);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("cultures/delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCulture(Guid id)
    {
        await _localizationService.DeleteCultureAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("import-files")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportFromFiles()
    {
        var resourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
        if (!Directory.Exists(resourcesPath)) return BadRequest("Resources directory not found.");

        var files = Directory.GetFiles(resourcesPath, "*.resx");
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var parts = fileName.Split('.'); // SharedResource.en-US

            string? group = null;
            string culture = "en-US";

            if (parts.Length >= 2)
            {
                culture = parts[parts.Length - 1];
                group = string.Join(".", parts.Take(parts.Length - 1));
            }
            else
            {
                group = parts[0];
            }

            var doc = XDocument.Load(file);
            var resources = doc.Root?.Elements("data")
                .GroupBy(e => e.Attribute("name")?.Value ?? "")
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToDictionary(
                    g => g.Key,
                    g => g.Last().Element("value")?.Value ?? ""
                ) ?? new();

            if (resources.Any())
            {
                await _localizationService.ImportResourcesAsync(resources, culture, group);
            }
        }

        return RedirectToAction(nameof(Index));
    }
}



