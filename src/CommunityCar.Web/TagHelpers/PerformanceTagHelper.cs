using CommunityCar.Application.Common.Interfaces.Services.SEO;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;

namespace CommunityCar.Web.TagHelpers;

[HtmlTargetElement("performance-optimizations", Attributes = "url")]
public class PerformanceTagHelper : TagHelper
{
    private readonly IPerformanceService _performanceService;
    private readonly Microsoft.AspNetCore.Mvc.ViewFeatures.IFileVersionProvider _fileVersionProvider;

    public PerformanceTagHelper(
        IPerformanceService performanceService,
        Microsoft.AspNetCore.Mvc.ViewFeatures.IFileVersionProvider fileVersionProvider)
    {
        _performanceService = performanceService;
        _fileVersionProvider = fileVersionProvider;
    }

    public string Url { get; set; } = "/";

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var criticalResources = await _performanceService.GetCriticalResourcesAsync(Url);
        
        output.TagName = null;
        
        var content = new StringBuilder();
        
        // Preconnect to common origins
        content.AppendLine("<link rel=\"preconnect\" href=\"https://fonts.googleapis.com\">");
        content.AppendLine("<link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin>");
        content.AppendLine("<link rel=\"preconnect\" href=\"https://cdn.jsdelivr.net\">");
        content.AppendLine("<link rel=\"preconnect\" href=\"https://cdnjs.cloudflare.com\">");
        
        var requestPathBase = ViewContext.HttpContext.Request.PathBase;

        // Preload critical resources
        foreach (var resource in criticalResources)
        {
            var type = resource.EndsWith(".css") ? "style" : resource.EndsWith(".js") ? "script" : "fetch";
            
            // Add version tracking if it's a local resource
            var finalResource = resource;
            if (resource.StartsWith("/"))
            {
                finalResource = _fileVersionProvider.AddFileVersionToPath(requestPathBase, resource);
            }
            
            content.AppendLine($"<link rel=\"preload\" href=\"{finalResource}\" as=\"{type}\">");
        }

        output.Content.SetHtmlContent(content.ToString());
    }
}



