namespace CommunityCar.Application.Features.Account.ViewModels.Security;

/// <summary>
/// ViewModel for security recommendations
/// </summary>
public class SecurityRecommendationVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // High, Medium, Low
    public string ActionUrl { get; set; } = string.Empty;
    public string ActionText { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int PointsImpact { get; set; } // How many points this would add to security score
}