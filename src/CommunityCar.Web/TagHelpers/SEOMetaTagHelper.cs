using CommunityCar.Application.Common.Interfaces.Services.SEO;
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
        var metaData = await _seoService.GenerateMetaDataAsync(PageType, EntityId);
        var structuredData = await _seoService.GenerateStructuredDataAsync(PageType, EntityId);

        output.TagName = null; // Don't render the <seo-meta> tag itself

        var content = new StringBuilder();
        
        // Basic Meta Tags
        content.AppendLine($"<title>{metaData.Title}</title>");
        content.AppendLine($"<meta name=\"description\" content=\"{metaData.Description}\" />");
        content.AppendLine($"<meta name=\"keywords\" content=\"{metaData.Keywords}\" />");
        
        if (!string.IsNullOrEmpty(metaData.CanonicalUrl))
        {
            content.AppendLine($"<link rel=\"canonical\" href=\"{metaData.CanonicalUrl}\" />");
        }

        // Open Graph
        content.AppendLine($"<meta property=\"og:title\" content=\"{metaData.OgTitle}\" />");
        content.AppendLine($"<meta property=\"og:description\" content=\"{metaData.OgDescription}\" />");
        content.AppendLine($"<meta property=\"og:type\" content=\"{metaData.OgType}\" />");
        content.AppendLine($"<meta property=\"og:site_name\" content=\"{metaData.SiteName}\" />");

        // Twitter
        content.AppendLine($"<meta name=\"twitter:card\" content=\"{metaData.TwitterCard}\" />");
        content.AppendLine($"<meta name=\"twitter:title\" content=\"{metaData.OgTitle}\" />");
        content.AppendLine($"<meta name=\"twitter:description\" content=\"{metaData.OgDescription}\" />");

        // Custom Meta Tags
        foreach (var tag in metaData.MetaTags)
        {
            content.AppendLine($"<meta name=\"{tag.Key}\" content=\"{tag.Value}\" />");
        }

        // Structured Data
        content.AppendLine("<script type=\"application/ld+json\">");
        content.AppendLine(structuredData);
        content.AppendLine("</script>");

        output.Content.SetHtmlContent(content.ToString());
    }
}



