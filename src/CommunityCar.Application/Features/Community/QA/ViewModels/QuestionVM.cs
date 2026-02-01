using System;
using System.Collections.Generic;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.QA.ViewModels;

public class QuestionVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? TitleAr { get; private set; }
    public string? BodyAr { get; private set; }
    public string Slug { get; set; } = string.Empty;
    public string AuthorName { get; set; } = "Anonymous";
    public Guid AuthorId { get; set; }
    public bool IsSolved { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public string? LastActivityBy { get; set; }
    public string BodyPreview => Body.Length > 200 ? Body.Substring(0, 197) + "..." : Body;
    
    // Enhanced properties
    public int AnswerCount { get; set; }
    public int VoteCount { get; set; }
    public int ViewCount { get; set; }
    public int VoteScore { get; set; }
    public bool IsPinned { get; set; }
    public bool IsLocked { get; set; }
    public string? CloseReason { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? ClosedByName { get; set; }
    
    // Difficulty and car details
    public DifficultyLevel Difficulty { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? CarEngine { get; set; }
    
    // Tags and categories
    public List<string> Tags { get; set; } = new();
    
    // Accepted answer info
    public Guid? AcceptedAnswerId { get; set; }
    public DateTime? SolvedAt { get; set; }
    
    // Author details
    public string? AuthorProfilePicture { get; set; }
    public int AuthorReputation { get; set; }
    public bool IsAuthorExpert { get; set; }
    
    // Engagement metrics
    public bool HasUserVoted { get; set; }
    public VoteType? UserVoteType { get; set; }
    public bool IsBookmarked { get; set; }
    
    // Time-based properties
    public string TimeAgo => GetTimeAgo(CreatedAt);
    public string LastActivityTimeAgo => GetTimeAgo(LastActivityAt);
    
    // Car display string
    public string CarDisplayName => GetCarDisplayName();
    
    // Difficulty display
    public string DifficultyDisplay => Difficulty.ToString();
    public string DifficultyColor => GetDifficultyColor();
    
    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;
        
        if (timeSpan.TotalMinutes < 1)
            return "just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes}m ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours}h ago";
        if (timeSpan.TotalDays < 30)
            return $"{(int)timeSpan.TotalDays}d ago";
        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)}mo ago";
        
        return $"{(int)(timeSpan.TotalDays / 365)}y ago";
    }
    
    private string GetCarDisplayName()
    {
        var parts = new List<string>();
        
        if (CarYear.HasValue)
            parts.Add(CarYear.ToString()!);
        if (!string.IsNullOrEmpty(CarMake))
            parts.Add(CarMake);
        if (!string.IsNullOrEmpty(CarModel))
            parts.Add(CarModel);
        if (!string.IsNullOrEmpty(CarEngine))
            parts.Add($"({CarEngine})");
            
        return parts.Count > 0 ? string.Join(" ", parts) : string.Empty;
    }
    
    private string GetDifficultyColor()
    {
        return Difficulty switch
        {
            DifficultyLevel.Beginner => "success",
            DifficultyLevel.Intermediate => "warning",
            DifficultyLevel.Advanced => "danger",
            DifficultyLevel.Expert => "dark",
            _ => "secondary"
        };
    }
}



