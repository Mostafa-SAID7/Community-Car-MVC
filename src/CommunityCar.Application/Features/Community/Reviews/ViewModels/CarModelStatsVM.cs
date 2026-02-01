namespace CommunityCar.Application.Features.Community.Reviews.ViewModels;

/// <summary>
/// Car model statistics view model
/// </summary>
public class CarModelStatsVM
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int ReviewCount { get; set; }
    public double AverageRating { get; set; }
    public double RecommendationPercentage { get; set; }
}