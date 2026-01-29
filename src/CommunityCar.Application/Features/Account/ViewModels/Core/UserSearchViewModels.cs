namespace CommunityCar.Application.Features.Account.ViewModels.Core;

public class UserSearchVM
{
    public string SearchTerm { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool ActiveOnly { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public List<UserCardVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasMore { get; set; }
}

public class UserSuggestionVM
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public int MutualConnectionsCount { get; set; }
    public List<string> MutualConnectionNames { get; set; } = new();
    public string SuggestionReason { get; set; } = string.Empty;
    public double SimilarityScore { get; set; }
    public bool IsFollowing { get; set; }
}

public class PeopleYouMayKnowVM
{
    public List<UserSuggestionVM> Suggestions { get; set; } = new();
    public int TotalSuggestions { get; set; }
    public bool HasMore { get; set; }
}