using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Social;

public class InterestAnalyticsVM
{
    public Guid UserId { get; set; }
    public int TotalInterests { get; set; }
    public Dictionary<string, int> InterestsByCategory { get; set; } = new();
    public List<ProfileInterestVM> TopInterests { get; set; } = new();
    public List<string> RecommendedInterests { get; set; } = new();
    public List<UserSuggestionVM> SimilarUsers { get; set; } = new();
}
