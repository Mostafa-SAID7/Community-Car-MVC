namespace CommunityCar.Application.Features.SEO.ViewModels;

public class SEOMetaDataVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string CanonicalUrl { get; set; } = string.Empty;
    public string OgTitle { get; set; } = string.Empty;
    public string OgDescription { get; set; } = string.Empty;
    public string OgType { get; set; } = string.Empty;
    public string TwitterCard { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;
    public Dictionary<string, string> MetaTags { get; set; } = new();
    public Dictionary<string, string> OpenGraphTags { get; set; } = new();
}