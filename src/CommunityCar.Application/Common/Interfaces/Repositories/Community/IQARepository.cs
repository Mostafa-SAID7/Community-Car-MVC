using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.QA;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IQARepository : IBaseRepository<Question>
{
    Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(Guid questionId);
    Task<Answer?> GetAnswerByIdAsync(Guid answerId);
    Task AddAnswerAsync(Answer answer);
    Task UpdateAnswerAsync(Answer answer);
    Task<Question?> GetQuestionByIdAsync(Guid id);
    Task<Question?> GetQuestionBySlugAsync(string slug);
}
