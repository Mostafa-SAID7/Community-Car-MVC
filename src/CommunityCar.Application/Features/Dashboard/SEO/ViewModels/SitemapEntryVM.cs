namespace CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

public class SitemapEntryVM
{
    public string Url { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public string ChangeFrequency { get; set; } = string.Empty;
    public double Priority { get; set; }
}