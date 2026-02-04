using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

public class UserProgressionVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int CurrentLevel { get; set; }
    public string CurrentLevelName { get; set; } = string.Empty;
    public int CurrentPoints { get; set; }
    public int PointsToNextLevel { get; set; }
    public double ProgressPercentage { get; set; }
    public DateTime LastLevelUp { get; set; }
    public List<LevelVM> AvailableLevels { get; set; } = new();
    public List<ProgressionMilestoneVM> Milestones { get; set; } = new();
    public List<ProgressionHistoryVM> RecentProgress { get; set; } = new();
    public Dictionary<string, int> SkillPoints { get; set; } = new();
}

public class LevelVM
{
    public int Level { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RequiredPoints { get; set; }
    public string IconUrl { get; set; } = string.Empty;
    public List<string> Perks { get; set; } = new();
    public bool IsUnlocked { get; set; }
    public bool IsCurrent { get; set; }
}

public class ProgressionMilestoneVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RequiredPoints { get; set; }
    public int RewardPoints { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double Progress { get; set; }
}

public class ProgressionHistoryVM
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public int PointsEarned { get; set; }
    public int PreviousLevel { get; set; }
    public int NewLevel { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsLevelUp { get; set; }
}