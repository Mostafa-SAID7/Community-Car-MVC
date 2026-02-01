namespace CommunityCar.Application.Features.Account.ViewModels.Social;

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