using CommunityCar.Application.Features.Community.QA.ViewModels;

namespace CommunityCar.Application.Features.Community.QA.DTOs;

public class QASearchResponse
{
    public List<QuestionVM> Questions { get; set; } = new();
    public PaginationInfo Pagination { get; set; } = new();
    public QASearchStats Stats { get; set; } = new();
    public List<string> AvailableTags { get; set; } = new();
    public List<string> AvailableCarMakes { get; set; } = new();
}

public class PaginationInfo
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public int StartItem { get; set; }
    public int EndItem { get; set; }
}

public class QASearchStats
{
    public int TotalQuestions { get; set; }
    public int SolvedQuestions { get; set; }
    public int OpenQuestions { get; set; }
    public int UnansweredQuestions { get; set; }
    public int PinnedQuestions { get; set; }
    public int LockedQuestions { get; set; }
    public int TotalAnswers { get; set; }
    public int TotalVotes { get; set; }
    public int TotalViews { get; set; }
}


