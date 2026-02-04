namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class TopPageVM
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int PageViews { get; set; }
    public int UniqueViews { get; set; }
    public double AverageTimeOnPage { get; set; }
    public double BounceRate { get; set; }
    public int Entrances { get; set; }
    public int Exits { get; set; }
}