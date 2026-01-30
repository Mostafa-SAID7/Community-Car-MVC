using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Gamification;

public class UserAchievement : BaseEntity
{
    public Guid UserId { get; private set; }
    public string AchievementId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string? TitleAr { get; private set; }
    public string? DescriptionAr { get; private set; }
    public int CurrentProgress { get; private set; }
    public int RequiredProgress { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsUnlocked => IsCompleted; // Alias for repository
    public DateTime? CompletedAt { get; private set; }
    public DateTime? UnlockedAt => CompletedAt; // Alias for repository
    public int RewardPoints { get; private set; }
    public string? RewardBadgeId { get; private set; }
    public string? AchievementType { get; private set; }

    public double ProgressPercentage => RequiredProgress > 0 ? (double)CurrentProgress / RequiredProgress * 100 : 0;
    public double Progress => ProgressPercentage; // Alias for repository

    public UserAchievement(Guid userId, string achievementId, string title, string description, 
                          int requiredProgress, int rewardPoints, string? rewardBadgeId = null)
    {
        UserId = userId;
        AchievementId = achievementId;
        Title = title;
        Description = description;
        RequiredProgress = requiredProgress;
        RewardPoints = rewardPoints;
        RewardBadgeId = rewardBadgeId;
        CurrentProgress = 0;
        IsCompleted = false;
    }

    public static UserAchievement Create(Guid userId, Guid achievementId, DateTime unlockedAt)
    {
        return new UserAchievement(userId, achievementId.ToString(), "Achievement", "Description", 1, 0)
        {
            IsCompleted = true,
            CompletedAt = unlockedAt,
            CurrentProgress = 1
        };
    }

    public static UserAchievement CreateInProgress(Guid userId, Guid achievementId, double progress)
    {
        var achievement = new UserAchievement(userId, achievementId.ToString(), "Achievement", "Description", 100, 0);
        achievement.UpdateProgress((int)progress);
        return achievement;
    }

    public void UpdateArabicContent(string? titleAr, string? descriptionAr)
    {
        TitleAr = titleAr;
        DescriptionAr = descriptionAr;
        Audit(UpdatedBy);
    }

    // EF Core constructor
    private UserAchievement() { }

    public void UpdateProgress(double progress)
    {
        CurrentProgress = (int)Math.Min(progress, RequiredProgress);
        
        if (CurrentProgress >= RequiredProgress && !IsCompleted)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
        }
        
        Audit(UpdatedBy);
    }

    public void Unlock(DateTime? unlockedAt = null)
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletedAt = unlockedAt ?? DateTime.UtcNow;
            CurrentProgress = RequiredProgress;
            Audit(UpdatedBy);
        }
    }

    public void IncrementProgress(int amount = 1)
    {
        UpdateProgress(CurrentProgress + amount);
    }
}