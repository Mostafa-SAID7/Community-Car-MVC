namespace CommunityCar.Application.Features.SEO.ViewModels;

public class RSSItemVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Guid { get; set; } = string.Empty;
}