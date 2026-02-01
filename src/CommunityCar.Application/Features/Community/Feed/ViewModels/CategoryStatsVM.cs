namespace CommunityCar.Application.Features.Community.Feed.ViewModels;


public class CategoryStatsVM
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}