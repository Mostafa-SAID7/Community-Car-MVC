using CommunityCar.Application.Common.Interfaces.Repositories.Base;
using CommunityCar.Domain.Entities.Community.QA;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Community;

public interface IQARepository : IBaseRepository<Question>
{
    Task<Question?> GetQuestionByIdAsync(Guid id);
    Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(Guid questionId);
    Task<Answer?> GetAnswerByIdAsync(Guid answerId);
    Task AddAnswerAsync(Answer answer);
    Task UpdateAnswerAsync(Answer answer);
    Task DeleteAnswerAsync(Answer answer);
}


