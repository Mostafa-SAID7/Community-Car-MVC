using System;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.QA.ViewModels;

public class AnswerVM
{
    public Guid Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public string AuthorName { get; set; } = "Anonymous";
    public Guid AuthorId { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    
    // Enhanced properties
    public int VoteCount { get; set; }
    public int VoteScore { get; set; }
    public int HelpfulCount { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? LastEditedAt { get; set; }
    public string? EditReason { get; set; }
    
    // Quality indicators
    public bool IsVerifiedByExpert { get; set; }
    public string? VerifiedByName { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNote { get; set; }
    
    // Author details
    public string? AuthorProfilePicture { get; set; }
    public int AuthorReputation { get; set; }
    public bool IsAuthorExpert { get; set; }
    public string? AuthorTitle { get; set; }
    
    // User interaction
    public bool HasUserVoted { get; set; }
    public VoteType? UserVoteType { get; set; }
    public bool HasUserMarkedHelpful { get; set; }
    
    // Time-based properties
    public string TimeAgo => GetTimeAgo(CreatedAt);
    public string? EditedTimeAgo => LastEditedAt.HasValue ? GetTimeAgo(LastEditedAt.Value) : null;
    public string? AcceptedTimeAgo => AcceptedAt.HasValue ? GetTimeAgo(AcceptedAt.Value) : null;
    public string? VerifiedTimeAgo => VerifiedAt.HasValue ? GetTimeAgo(VerifiedAt.Value) : null;
    
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
}
