using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Application.Features.Shared.ViewModels;

namespace CommunityCar.Application.Features.Community.QA.ViewModels;

public class QASearchVM
{
    // Search parameters
    public string? SearchTerm { get; set; }
    public string? Query { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool? IsAnswered { get; set; }
    public bool? HasAcceptedAnswer { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? MinVotes { get; set; }
    public int? MaxVotes { get; set; }
    public int? MinAnswers { get; set; }
    public int? MaxAnswers { get; set; }
    public int? MinViews { get; set; }
    public int? MaxViews { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public QAStatus? Status { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
    public int? CarYear { get; set; }
    public bool? IsPinned { get; set; }
    public bool? IsLocked { get; set; }
    public Guid? AuthorId { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? LastActivityAfter { get; set; }
    public DateTime? LastActivityBefore { get; set; }
    
    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public QASortBy SortBy { get; set; } = QASortBy.Newest;
    public string SortOrder { get; set; } = "desc";
    
    // Results
    public List<QuestionVM> Questions { get; set; } = new();
    public List<QuestionVM> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public PaginationVM Pagination { get; set; } = new();
    public QASearchStats Stats { get; set; } = new();
    public List<string> AvailableTags { get; set; } = new();
    public List<string> AvailableCarMakes { get; set; } = new();
}