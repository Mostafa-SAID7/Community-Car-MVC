namespace CommunityCar.Application.Features.SEO.ViewModels;

public class SitemapUrlVM
{
    public string Url { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public string ChangeFrequency { get; set; } = string.Empty;
    public decimal Priority { get; set; }
}