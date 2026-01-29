namespace CommunityCar.Application.Features.Account.ViewModels.Social;

using CommunityCar.Application.Features.Shared.ViewModels;

public class ProfileInterestVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid InterestId { get; set; }
    public string InterestName { get; set; } = string.Empty;
    public string? InterestDescription { get; set; }
    public string? Category { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CategoryIcon { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public int UsersWithInterestCount { get; set; }
}

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

public class InterestSuggestionVM
{
    public Guid InterestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int PopularityScore { get; set; }
    public string RecommendationReason { get; set; } = string.Empty;
    public bool IsAdded { get; set; }
}

