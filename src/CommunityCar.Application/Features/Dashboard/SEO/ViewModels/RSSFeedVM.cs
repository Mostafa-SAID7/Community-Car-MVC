namespace CommunityCar.Application.Features.SEO.ViewModels;

public class RSSFeedVM
{
    public string FeedType { get; set; } = "RSS";
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public DateTime LastBuildDate { get; set; }
    public List<RSSItemVM> Items { get; set; } = new();
}