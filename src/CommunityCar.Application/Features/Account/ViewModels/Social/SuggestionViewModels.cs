namespace CommunityCar.Application.Features.Account.ViewModels.Social;

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