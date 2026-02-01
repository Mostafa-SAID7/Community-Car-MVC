namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

public class LeaderboardEntryVM
{
    public int Rank { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public string? Trend { get; set; } 
}