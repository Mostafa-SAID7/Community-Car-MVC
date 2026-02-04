using CommunityCar.Application.Common.Interfaces.Services.Dashboard.SEO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace CommunityCar.Web.TagHelpers;

[HtmlTargetElement("seo-meta", Attributes = "page-type")]
public class SEOMetaTagHelper : TagHelper
{
    private readonly ISEOService _seoService;

    public SEOMetaTagHelper(ISEOService seoService)
    {
        _seoService = seoService;
    }

    public string PageType { get; set; } = "Home";
    public Guid? EntityId { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // Construct URL from PageType and EntityId
        var url = ConstructUrl(PageType, EntityId);
        
        var metaData = await _seoService.GenerateMetaDataAsync(url);
        var structuredData = await _seoService.GenerateStructuredDataAsync(url);

        output.TagName = null; // Don't render the <seo-meta> tag itself

        var content = new StringBuilder();
        
        // Basic Meta Tags
        content.AppendLine($"<title>{GetValueOrDefault(metaData, "Title", "Community Car")}</title>");
        content.AppendLine($"<meta name=\"description\" content=\"{GetValueOrDefault(metaData, "Description", "Community Car Platform")}\" />");
        content.AppendLine($"<meta name=\"keywords\" content=\"{GetValueOrDefault(metaData, "Keywords", "community, car, automotive")}\" />");
        
        var canonicalUrl = GetValueOrDefault(metaData, "CanonicalUrl", "");
        if (!string.IsNullOrEmpty(canonicalUrl))
        {
            content.AppendLine($"<link rel=\"canonical\" href=\"{canonicalUrl}\" />");
        }

        // Open Graph
        content.AppendLine($"<meta property=\"og:title\" content=\"{GetValueOrDefault(metaData, "OgTitle", GetValueOrDefault(metaData, "Title", "Community Car"))}\" />");
        content.AppendLine($"<meta property=\"og:description\" content=\"{GetValueOrDefault(metaData, "OgDescription", GetValueOrDefault(metaData, "Description", "Community Car Platform"))}\" />");
        content.AppendLine($"<meta property=\"og:type\" content=\"{GetValueOrDefault(metaData, "OgType", "website")}\" />");
        content.AppendLine($"<meta property=\"og:site_name\" content=\"{GetValueOrDefault(metaData, "SiteName", "Community Car")}\" />");

        // Twitter
        content.AppendLine($"<meta name=\"twitter:card\" content=\"{GetValueOrDefault(metaData, "TwitterCard", "summary")}\" />");
        content.AppendLine($"<meta name=\"twitter:title\" content=\"{GetValueOrDefault(metaData, "OgTitle", GetValueOrDefault(metaData, "Title", "Community Car"))}\" />");
        content.AppendLine($"<meta name=\"twitter:description\" content=\"{GetValueOrDefault(metaData, "OgDescription", GetValueOrDefault(metaData, "Description", "Community Car Platform"))}\" />");

        // Structured Data
        content.AppendLine("<script type=\"application/ld+json\">");
        content.AppendLine(structuredData.ToString());
        content.AppendLine("</script>");

        output.Content.SetHtmlContent(content.ToString());
    }

    private string ConstructUrl(string pageType, Guid? entityId)
    {
        // Simple URL construction based on page type and entity ID
        var baseUrl = "/";
        
        return pageType.ToLower() switch
        {
            "home" => baseUrl,
            "post" => entityId.HasValue ? $"/posts/{entityId}" : "/posts",
            "question" => entityId.HasValue ? $"/qa/{entityId}" : "/qa",
            "guide" => entityId.HasValue ? $"/guides/{entityId}" : "/guides",
            "event" => entityId.HasValue ? $"/events/{entityId}" : "/events",
            "profile" => entityId.HasValue ? $"/profile/{entityId}" : "/profile",
            _ => baseUrl
        };
    }

    private string GetValueOrDefault(Dictionary<string, object> dictionary, string key, string defaultValue)
    {
        return dictionary.TryGetValue(key, out var value) ? value?.ToString() ?? defaultValue : defaultValue;
    }
}



