using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Account.Profile;

public class UserInterest : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string SubCategory { get; private set; } = string.Empty;
    public string InterestType { get; private set; } = string.Empty; // CarMake, CarModel, Topic, Tag, etc.
    public string InterestValue { get; private set; } = string.Empty;
    public double Score { get; private set; } = 0.0;
    public int InteractionCount { get; private set; } = 0;
    public DateTime LastInteraction { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Source { get; private set; } // How this interest was detected

    public UserInterest(
        Guid userId,
        string category,
        string subCategory,
        string interestType,
        string interestValue,
        double initialScore = 1.0,
        string? source = null)
    {
        UserId = userId;
        Category = category;
        SubCategory = subCategory;
        InterestType = interestType;
        InterestValue = interestValue;
        Score = initialScore;
        InteractionCount = 1;
        LastInteraction = DateTime.UtcNow;
        Source = source;
    }

    private UserInterest() { }

    public void IncrementScore(double increment = 1.0)
    {
        Score += increment;
        InteractionCount++;
        LastInteraction = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void DecrementScore(double decrement = 0.5)
    {
        Score = Math.Max(0, Score - decrement);
        LastInteraction = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void SetScore(double newScore)
    {
        Score = Math.Max(0, newScore);
        LastInteraction = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        Audit(UpdatedBy);
    }

    public void UpdateSource(string source)
    {
        Source = source;
        Audit(UpdatedBy);
    }

    public bool IsStale(int daysThreshold = 30)
    {
        return DateTime.UtcNow.Subtract(LastInteraction).TotalDays > daysThreshold;
    }

    public string GetDisplayName()
    {
        return InterestType switch
        {
            "CarMake" => $"{InterestValue} vehicles",
            "CarModel" => $"{InterestValue} model",
            "Topic" => InterestValue,
            "Tag" => $"#{InterestValue}",
            "Location" => $"{InterestValue} area",
            "EventType" => $"{InterestValue} events",
            _ => InterestValue
        };
    }
}