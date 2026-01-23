using CommunityCar.Application.Common.Interfaces.Services.Localization;
using CommunityCar.Domain.Entities.Localization;
using Microsoft.AspNetCore.Authorization;
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
        return View(cultures);
    }

    [HttpGet("resources")]
    public async Task<IActionResult> Resources(string? culture, string? group)
    {
        var resources = await _localizationService.GetAllResourcesAsync(culture, group);
        ViewBag.Cultures = await _localizationService.GetSupportedCulturesAsync();
        ViewBag.SelectedCulture = culture;
        ViewBag.SelectedGroup = group;
        return View("Resources", resources);
    }

    [HttpGet("resources/add")]
    public async Task<IActionResult> AddResource()
    {
        ViewBag.Cultures = await _localizationService.GetSupportedCulturesAsync();
        return View("EditResource", new LocalizationResource());
    }

    [HttpGet("resources/edit/{id:guid}")]
    public async Task<IActionResult> EditResource(Guid id)
    {
        var resource = (await _localizationService.GetAllResourcesAsync()).FirstOrDefault(r => r.Id == id);
        if (resource == null) return NotFound();

        ViewBag.Cultures = await _localizationService.GetSupportedCulturesAsync();
        return View("EditResource", resource);
    }

    [HttpPost("resources/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveResource(LocalizationResource resource)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Cultures = await _localizationService.GetSupportedCulturesAsync();
            return View("EditResource", resource);
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
        return View("EditCulture", new LocalizationCulture());
    }

    [HttpPost("cultures/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCulture(LocalizationCulture culture)
    {
        if (!ModelState.IsValid)
        {
            return View("EditCulture", culture);
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
                .ToDictionary(
                    e => e.Attribute("name")?.Value ?? "",
                    e => e.Element("value")?.Value ?? ""
                ) ?? new();

            if (resources.Any())
            {
                await _localizationService.ImportResourcesAsync(resources, culture, group);
            }
        }

        return RedirectToAction(nameof(Index));
    }
}
