using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.QA.ViewModels;

public class QuestionVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? BodyAr { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePicture { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsSolved { get; set; }
    public bool IsPinned { get; set; }
    public bool IsLocked { get; set; }
    public bool IsBookmarked { get; set; }
    public string? LockReason { get; set; }
    public Guid? LockedBy { get; set; }
    public DateTime? LockedAt { get; set; }
    public int AnswerCount { get; set; }
    public int VoteCount { get; set; }
    public int VoteScore { get; set; }
    public int ViewCount { get; set; }
    public Guid? AcceptedAnswerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public string? LastActivityBy { get; set; }
    public string? LastActivityTimeAgo { get; set; }
    
    // Car-specific properties
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public string? CarEngine { get; set; }
    
    // Difficulty and complexity
    public DifficultyLevel Difficulty { get; set; }
    public string DifficultyDisplay => Difficulty.ToString();
    
    // Helper properties
    public string TimeAgo
    {
        get
        {
            var timeAgo = DateTime.UtcNow - CreatedAt;
            return timeAgo.TotalDays > 7 ? CreatedAt.ToString("MMM dd, yyyy") :
                   timeAgo.TotalDays >= 1 ? $"{(int)timeAgo.TotalDays} days ago" :
                   timeAgo.TotalHours >= 1 ? $"{(int)timeAgo.TotalHours} hours ago" :
                   "Just now";
        }
    }
    
    public string StatusText
    {
        get
        {
            if (IsLocked) return "Locked";
            if (IsSolved) return "Solved";
            if (IsPinned) return "Pinned";
            if (AnswerCount == 0) return "Unanswered";
            return "Open";
        }
    }
    
    public string StatusColor
    {
        get
        {
            if (IsLocked) return "red";
            if (IsSolved) return "green";
            if (IsPinned) return "blue";
            if (AnswerCount == 0) return "orange";
            return "gray";
        }
    }
    
    public bool HasAcceptedAnswer => AcceptedAnswerId.HasValue;
    public bool IsEdited => UpdatedAt.HasValue && UpdatedAt > CreatedAt.AddMinutes(5);
    public string CarDisplayName => !string.IsNullOrEmpty(CarMake) && !string.IsNullOrEmpty(CarModel) 
        ? $"{CarYear} {CarMake} {CarModel}".Trim()
        : !string.IsNullOrEmpty(CarMake) 
            ? CarMake 
            : string.Empty;
}