namespace CommunityCar.Application.Features.Dashboard.SEO.ViewModels;

public class RSSFeedVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public DateTime LastBuildDate { get; set; }
    public int MaxItems { get; set; } = 20;
    public List<RSSItemVM> Items { get; set; } = new();
}