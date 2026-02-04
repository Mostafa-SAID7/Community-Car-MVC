namespace CommunityCar.Application.Features.Account.ViewModels.Security;

/// <summary>
/// ViewModel for security score
/// </summary>
public class SecurityScoreVM
{
    public int Score { get; set; } // 0-100
    public string Grade { get; set; } = string.Empty; // A, B, C, D, F
    public string Status { get; set; } = string.Empty; // Excellent, Good, Fair, Poor
    public List<SecurityRecommendationVM> Recommendations { get; set; } = new();
    public Dictionary<string, bool> SecurityChecks { get; set; } = new();
    public DateTime LastCalculated { get; set; }
}