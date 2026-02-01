namespace CommunityCar.Application.Features.SEO.ViewModels;

public class SitemapVM
{
    public DateTime GeneratedAt { get; set; }
    public List<SitemapUrlVM> Urls { get; set; } = new();
    public int TotalUrls { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public string ChangeFrequency { get; set; } = "monthly";
    public double Priority { get; set; } = 0.5;
}