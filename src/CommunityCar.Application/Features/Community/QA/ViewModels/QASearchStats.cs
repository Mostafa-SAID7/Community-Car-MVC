namespace CommunityCar.Application.Features.Community.QA.ViewModels;

public class QASearchStats
{
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public int UnansweredQuestions { get; set; }
    public int QuestionsWithAcceptedAnswers { get; set; }
    public int SolvedQuestions { get; set; }
    public int OpenQuestions { get; set; }
    public int PinnedQuestions { get; set; }
    public int LockedQuestions { get; set; }
    public int TotalAnswers { get; set; }
    public int TotalVotes { get; set; }
    public int TotalViews { get; set; }
    public double AnswerRate => TotalQuestions > 0 ? (double)AnsweredQuestions / TotalQuestions * 100 : 0;
    public double AcceptanceRate => AnsweredQuestions > 0 ? (double)QuestionsWithAcceptedAnswers / AnsweredQuestions * 100 : 0;
    public Dictionary<string, int> QuestionsByCategory { get; set; } = new();
    public Dictionary<string, int> QuestionsByTag { get; set; } = new();
}