using System;
using System.Collections.Generic;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Domain.Entities.Shared;

namespace CommunityCar.Domain.Entities.Community.QA;

public class Question : AggregateRoot
{
    public string Title { get; private set; }
    public string Body { get; private set; }
    
    // Arabic Localization
    public string? TitleAr { get; private set; }
    public string? BodyAr { get; private set; }
    
    public Guid AuthorId { get; private set; }
    public bool IsSolved { get; private set; }
    public Guid? AcceptedAnswerId { get; private set; }
    public DateTime? SolvedAt { get; private set; }
    
    // Additional properties for enhanced QA
    public int ViewCount { get; private set; }
    public int AnswerCount { get; private set; }
    public int VoteScore { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public string? LastActivityBy { get; private set; }
    public bool IsPinned { get; private set; }
    public bool IsLocked { get; private set; }
    public string? CloseReason { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public Guid? ClosedBy { get; private set; }
    
    // Difficulty level for car-related questions
    public DifficultyLevel Difficulty { get; private set; }
    
    // Car-specific properties
    public string? CarMake { get; private set; }
    public string? CarModel { get; private set; }
    public int? CarYear { get; private set; }
    public string? CarEngine { get; private set; }
    
    // Collections
    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    public Question(string title, string body, Guid authorId, DifficultyLevel difficulty = DifficultyLevel.Beginner)
    {
        Title = title;
        Body = body;
        AuthorId = authorId;
        IsSolved = false;
        ViewCount = 0;
        AnswerCount = 0;
        VoteScore = 0;
        LastActivityAt = DateTime.UtcNow;
        LastActivityBy = authorId.ToString();
        IsPinned = false;
        IsLocked = false;
        Difficulty = difficulty;
    }

    private Question() { }

    public void MarkAsSolved(Guid acceptedAnswerId)
    {
        IsSolved = true;
        AcceptedAnswerId = acceptedAnswerId;
        SolvedAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void UpdateAnswerCount(int count)
    {
        AnswerCount = count;
        LastActivityAt = DateTime.UtcNow;
    }

    public void UpdateVoteScore(int score)
    {
        VoteScore = score;
        LastActivityAt = DateTime.UtcNow;
    }

    public void Pin()
    {
        IsPinned = true;
        Audit(UpdatedBy);
    }

    public void Unpin()
    {
        IsPinned = false;
        Audit(UpdatedBy);
    }

    public void Lock(string reason, Guid lockedBy)
    {
        IsLocked = true;
        CloseReason = reason;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = lockedBy;
        Audit(UpdatedBy);
    }

    public void Unlock()
    {
        IsLocked = false;
        CloseReason = null;
        ClosedAt = null;
        ClosedBy = null;
        Audit(UpdatedBy);
    }

    public void SetCarDetails(string? make, string? model, int? year, string? engine)
    {
        CarMake = make;
        CarModel = model;
        CarYear = year;
        CarEngine = engine;
        Audit(UpdatedBy);
    }

    public void AddTag(string tag)
    {
        if (!_tags.Contains(tag.ToLowerInvariant()))
        {
            _tags.Add(tag.ToLowerInvariant());
        }
    }

    public void RemoveTag(string tag)
    {
        _tags.Remove(tag.ToLowerInvariant());
    }

    public void ClearTags()
    {
        _tags.Clear();
    }

    public void UpdateActivity(string activityBy)
    {
        LastActivityAt = DateTime.UtcNow;
        LastActivityBy = activityBy;
    }

    public void UpdateArabicContent(string? titleAr, string? bodyAr)
    {
        TitleAr = titleAr;
        BodyAr = bodyAr;
        Audit(UpdatedBy);
    }

    public string CarDisplayName => 
        !string.IsNullOrEmpty(CarMake) && !string.IsNullOrEmpty(CarModel) 
            ? $"{CarYear} {CarMake} {CarModel}".Trim()
            : string.Empty;
}
