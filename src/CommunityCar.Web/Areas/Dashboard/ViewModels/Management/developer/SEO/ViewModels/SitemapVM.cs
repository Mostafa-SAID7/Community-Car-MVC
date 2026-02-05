namespace CommunityCar.Application.Features.Dashboard.Management.developer.SEO.ViewModels;

public class SitemapVM
{
    public string Url { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public string ChangeFrequency { get; set; } = string.Empty;
    public double Priority { get; set; }
    public List<SitemapEntryVM> Entries { get; set; } = new();
}