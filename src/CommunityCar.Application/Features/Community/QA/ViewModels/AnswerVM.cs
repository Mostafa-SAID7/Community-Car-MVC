namespace CommunityCar.Application.Features.Community.QA.ViewModels;

public class AnswerVM
{
    public Guid Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? BodyAr { get; set; }
    public Guid QuestionId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePicture { get; set; }
    public bool IsAccepted { get; set; }
    public bool IsVerified { get; set; }
    public bool IsVerifiedByExpert { get; set; }
    public Guid? VerifiedBy { get; set; }
    public string? VerifiedByName { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNote { get; set; }
    public int VoteCount { get; set; }
    public int VoteScore { get; set; }
    public int HelpfulCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
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
    
    public bool IsEdited => UpdatedAt.HasValue && UpdatedAt > CreatedAt.AddMinutes(5);
    public string VerificationText => IsVerified ? $"Verified by {VerifiedByName}" : string.Empty;
}