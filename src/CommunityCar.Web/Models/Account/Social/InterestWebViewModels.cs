namespace CommunityCar.Web.Models.Account.Social;

public class UserInterestWebVM
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

public class InterestsDashboardWebVM
{
    public Guid UserId { get; set; }
    public int TotalInterests { get; set; }
    public Dictionary<string, int> InterestsByCategory { get; set; } = new();
    public List<UserInterestWebVM> TopInterests { get; set; } = new();
    public List<InterestSuggestionWebVM> RecommendedInterests { get; set; } = new();
    public List<UserSuggestionWebVM> SimilarUsers { get; set; } = new();
    public List<CategoryWebVM> Categories { get; set; } = new();
}

public class InterestSuggestionWebVM
{
    public Guid InterestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int PopularityScore { get; set; }
    public string RecommendationReason { get; set; } = string.Empty;
    public bool IsAdded { get; set; }
}

public class CategoryWebVM
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int InterestCount { get; set; }
    public List<UserInterestWebVM> Interests { get; set; } = new();
}