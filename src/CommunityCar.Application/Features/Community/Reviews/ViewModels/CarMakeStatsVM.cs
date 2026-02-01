namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Car make statistics view model
/// </summary>
public class CarMakeStatsVM
{
    public string Make { get; set; } = string.Empty;
    public int ReviewCount { get; set; }
    public double AverageRating { get; set; }
    public double RecommendationPercentage { get; set; }
}