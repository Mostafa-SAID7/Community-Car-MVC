namespace CommunityCar.Domain.Enums.Community;

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
    Open,
    Solved,
    Unanswered,
    Pinned,
    Locked
}