namespace CommunityCar.Web.Areas.Identity.ViewModels.Gamification;

public class QuestVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int RewardPoints { get; set; }
    public string? RewardBadge { get; set; }
    public string Status { get; set; } = string.Empty; // Available, InProgress, Completed, Expired
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int Progress { get; set; }
    public int MaxProgress { get; set; }
    public string ProgressDescription { get; set; } = string.Empty;
    public List<QuestRequirementVM> Requirements { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class QuestRequirementVM
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CurrentValue { get; set; }
    public int RequiredValue { get; set; }
    public bool IsCompleted { get; set; }
}
