using CommunityCar.Application.Features.Community.QA.ViewModels;
using CommunityCar.Domain.Enums.Community;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Common.Interfaces.Services.Community.QA;

public interface IQAService
{
    Task<QASearchVM> SearchQuestionsAsync(QASearchVM request);
    Task<IEnumerable<QuestionVM>> GetAllQuestionsAsync();
    Task<QuestionVM?> GetQuestionByIdAsync(Guid id);
    Task<QuestionVM?> GetQuestionBySlugAsync(string slug);
    Task<IEnumerable<AnswerVM>> GetAnswersByQuestionIdAsync(Guid questionId);
    Task<QuestionVM> CreateQuestionAsync(string title, string body, Guid authorId, string? titleAr = null, string? bodyAr = null, DifficultyLevel difficulty = DifficultyLevel.Beginner, string? carMake = null, string? carModel = null, int? carYear = null, string? carEngine = null, List<string>? tags = null);
    Task<AnswerVM> CreateAnswerAsync(Guid questionId, string body, Guid authorId, string? bodyAr = null);
    Task MarkAsSolvedAsync(Guid questionId, Guid acceptedAnswerId);
    Task AcceptAnswerAsync(Guid answerId);
    Task VoteAsync(Guid entityId, EntityType entityType, Guid userId, VoteType voteType);
    Task<int> GetVoteCountAsync(Guid entityId, EntityType entityType);
    Task MarkHelpfulAsync(Guid answerId, Guid userId);
    Task BookmarkQuestionAsync(Guid questionId, Guid userId);
    Task UnbookmarkQuestionAsync(Guid questionId, Guid userId);
    Task IncrementViewCountAsync(Guid questionId, Guid? userId = null, string? ipAddress = null, string? userAgent = null);
    Task PinQuestionAsync(Guid questionId, Guid moderatorId);
    Task UnpinQuestionAsync(Guid questionId, Guid moderatorId);
    Task LockQuestionAsync(Guid questionId, string reason, Guid moderatorId);
    Task UnlockQuestionAsync(Guid questionId, Guid moderatorId);
    Task VerifyAnswerAsync(Guid answerId, Guid expertId, string? note = null);
    Task RemoveAnswerVerificationAsync(Guid answerId, Guid expertId);
    Task<List<string>> GetPopularTagsAsync(int count = 20);
    Task<List<string>> GetAvailableCarMakesAsync();
    Task<QASearchStats> GetQAStatsAsync();
}