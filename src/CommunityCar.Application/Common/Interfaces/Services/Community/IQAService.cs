using CommunityCar.Application.Features.QA.ViewModels;
using CommunityCar.Application.Features.QA.DTOs;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Common.Interfaces.Services.Community;

public interface IQAService
{
    // Enhanced search and filtering
    Task<QASearchResponse> SearchQuestionsAsync(QASearchRequest request);
    Task<IEnumerable<QuestionVM>> GetAllQuestionsAsync();
    Task<QuestionVM?> GetQuestionByIdAsync(Guid id);
    Task<IEnumerable<AnswerVM>> GetAnswersByQuestionIdAsync(Guid questionId);
    
    // Question management
    Task<QuestionVM> CreateQuestionAsync(string title, string body, Guid authorId, DifficultyLevel difficulty = DifficultyLevel.Beginner, string? carMake = null, string? carModel = null, int? carYear = null, string? carEngine = null, List<string>? tags = null);
    Task<AnswerVM> CreateAnswerAsync(Guid questionId, string body, Guid authorId);
    Task MarkAsSolvedAsync(Guid questionId, Guid acceptedAnswerId);
    Task AcceptAnswerAsync(Guid answerId);
    
    // Voting and engagement
    Task VoteAsync(Guid entityId, EntityType entityType, Guid userId, VoteType voteType);
    Task<int> GetVoteCountAsync(Guid entityId, EntityType entityType);
    Task MarkHelpfulAsync(Guid answerId, Guid userId);
    Task BookmarkQuestionAsync(Guid questionId, Guid userId);
    Task UnbookmarkQuestionAsync(Guid questionId, Guid userId);
    
    // View tracking
    Task IncrementViewCountAsync(Guid questionId, Guid? userId = null, string? ipAddress = null, string? userAgent = null);
    
    // Moderation
    Task PinQuestionAsync(Guid questionId, Guid moderatorId);
    Task UnpinQuestionAsync(Guid questionId, Guid moderatorId);
    Task LockQuestionAsync(Guid questionId, string reason, Guid moderatorId);
    Task UnlockQuestionAsync(Guid questionId, Guid moderatorId);
    
    // Expert verification
    Task VerifyAnswerAsync(Guid answerId, Guid expertId, string? note = null);
    Task RemoveAnswerVerificationAsync(Guid answerId, Guid expertId);
    
    // Statistics and analytics
    Task<List<string>> GetPopularTagsAsync(int count = 20);
    Task<List<string>> GetAvailableCarMakesAsync();
    Task<QASearchStats> GetQAStatsAsync();
}