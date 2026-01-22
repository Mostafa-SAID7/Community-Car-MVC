using CommunityCar.Application.Common.Interfaces.Services.SEO;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace CommunityCar.Web.TagHelpers;

[HtmlTargetElement("performance-optimizations", Attributes = "url")]
public class PerformanceTagHelper : TagHelper
{
    private readonly IPerformanceService _performanceService;

    public PerformanceTagHelper(IPerformanceService performanceService)
    {
        _performanceService = performanceService;
    }

    public string Url { get; set; } = "/";

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

        // Preload critical resources
        foreach (var resource in criticalResources)
        {
            var type = resource.EndsWith(".css") ? "style" : resource.EndsWith(".js") ? "script" : "fetch";
            content.AppendLine($"<link rel=\"preload\" href=\"{resource}\" as=\"{type}\">");
        }

        output.Content.SetHtmlContent(content.ToString());
    }
}
