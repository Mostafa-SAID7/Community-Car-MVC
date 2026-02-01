namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Available filters view model
/// </summary>
public class ReviewsFiltersVM
{
    public List<string> CarMakes { get; set; } = new();
    public List<string> CarModels { get; set; } = new();
    public List<int> CarYears { get; set; } = new();
    public List<string> TargetTypes { get; set; } = new();
    public int MinYear { get; set; }
    public int MaxYear { get; set; }
}