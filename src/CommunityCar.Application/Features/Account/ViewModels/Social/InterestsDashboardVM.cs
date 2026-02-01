using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Account.ViewModels.Social;

public class InterestsDashboardVM
{
    public Guid UserId { get; set; }
    public int TotalInterests { get; set; }
    public Dictionary<string, int> InterestsByCategory { get; set; } = new();
    public List<ProfileInterestVM> TopInterests { get; set; } = new();
    public List<InterestSuggestionVM> RecommendedInterests { get; set; } = new();
    public List<UserSuggestionVM> SimilarUsers { get; set; } = new();
    public List<CategoryVM> Categories { get; set; } = new();
}