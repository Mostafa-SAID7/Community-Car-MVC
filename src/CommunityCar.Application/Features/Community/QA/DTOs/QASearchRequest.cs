using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Community.QA.DTOs;

public class QASearchRequest
{
    public string? SearchTerm { get; set; }
    public QASortBy SortBy { get; set; } = QASortBy.Newest;
    public DifficultyLevel? Difficulty { get; set; }
    public QAStatus? Status { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool? IsPinned { get; set; }
    public bool? IsLocked { get; set; }
    public Guid? AuthorId { get; set; }
    
    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    
    // Date filters
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? LastActivityAfter { get; set; }
    public DateTime? LastActivityBefore { get; set; }
    
    // Vote and engagement filters
    public int? MinVotes { get; set; }
    public int? MaxVotes { get; set; }
    public int? MinAnswers { get; set; }
    public int? MaxAnswers { get; set; }
    public int? MinViews { get; set; }
    public int? MaxViews { get; set; }
}

public enum QASortBy
{
    Newest,
    Oldest,
    MostVotes,
    LeastVotes,
    MostAnswers,
    LeastAnswers,
    MostViews,
    LeastViews,
    RecentActivity,
    Relevance
}

public enum QAStatus
{
    All,
    Open,
    Solved,
    Unanswered,
    Pinned,
    Locked
}


