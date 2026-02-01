namespace CommunityCar.Application.Features.Community.Feed.ViewModels;


public class TagStatsVM
{
    public string Tag { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}