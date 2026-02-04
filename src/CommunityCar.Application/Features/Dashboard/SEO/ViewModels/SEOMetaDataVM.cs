using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

public class SEOMetaDataVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Canonical { get; set; } = string.Empty;
    public string OgTitle { get; set; } = string.Empty;
    public string OgDescription { get; set; } = string.Empty;
    public string OgImage { get; set; } = string.Empty;
    public string OgUrl { get; set; } = string.Empty;
    public string TwitterCard { get; set; } = string.Empty;
    public string TwitterTitle { get; set; } = string.Empty;
    public string TwitterDescription { get; set; } = string.Empty;
    public string TwitterImage { get; set; } = string.Empty;
    public Dictionary<string, string> CustomMeta { get; set; } = new();
}