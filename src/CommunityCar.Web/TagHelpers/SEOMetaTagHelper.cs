using CommunityCar.Web.Areas.Dashboard.Interfaces.Services.SEO;
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
        
        // Use available methods from ISEOService
        var metaDescription = await _seoService.GenerateMetaDescriptionAsync($"{PageType} page");
        var keywords = await _seoService.GenerateKeywordsAsync($"{PageType} page");
        var seoScore = await _seoService.GetSEOScoreAsync(url);

        output.TagName = null; // Don't render the <seo-meta> tag itself

        var content = new StringBuilder();
        
        // Basic Meta Tags
        content.AppendLine($"<title>Community Car - {PageType}</title>");
        content.AppendLine($"<meta name=\"description\" content=\"{metaDescription}\" />");
        content.AppendLine($"<meta name=\"keywords\" content=\"{string.Join(", ", keywords)}\" />");
        
        content.AppendLine($"<link rel=\"canonical\" href=\"{url}\" />");

        // Open Graph
        content.AppendLine($"<meta property=\"og:title\" content=\"Community Car - {PageType}\" />");
        content.AppendLine($"<meta property=\"og:description\" content=\"{metaDescription}\" />");
        content.AppendLine($"<meta property=\"og:type\" content=\"website\" />");
        content.AppendLine($"<meta property=\"og:site_name\" content=\"Community Car\" />");

        // Twitter
        content.AppendLine($"<meta name=\"twitter:card\" content=\"summary\" />");
        content.AppendLine($"<meta name=\"twitter:title\" content=\"Community Car - {PageType}\" />");
        content.AppendLine($"<meta name=\"twitter:description\" content=\"{metaDescription}\" />");

        // Simple structured data
        var structuredData = $@"{{
            ""@context"": ""https://schema.org"",
            ""@type"": ""WebPage"",
            ""name"": ""Community Car - {PageType}"",
            ""description"": ""{metaDescription}"",
            ""url"": ""{url}""
        }}";

        content.AppendLine("<script type=\"application/ld+json\">");
        content.AppendLine(structuredData);
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
}




