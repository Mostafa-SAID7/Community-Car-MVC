using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

public class LeaderboardVM
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public List<LeaderboardEntryVM> Entries { get; set; } = new();
    public int TotalParticipants { get; set; }
    public string TimeFrame { get; set; } = string.Empty; // Daily, Weekly, Monthly, All-time
    public LeaderboardEntryVM? CurrentUserEntry { get; set; }
}
