using CommunityCar.Application.Common.Interfaces.Repositories.Community;
using CommunityCar.Domain.Entities.Community.QA;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class QARepository : BaseRepository<Question>, IQARepository
{
    public QARepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Question?> GetQuestionByIdAsync(Guid id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(Guid questionId)
    {
        return await Context.Answers
            .Where(a => a.QuestionId == questionId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Answer?> GetAnswerByIdAsync(Guid answerId)
    {
        return await Context.Answers.FindAsync(answerId);
    }

    public async Task AddAnswerAsync(Answer answer)
    {
        await Context.Answers.AddAsync(answer);
    }

    public Task UpdateAnswerAsync(Answer answer)
    {
        Context.Answers.Update(answer);
        return Task.CompletedTask;
    }

    public Task DeleteAnswerAsync(Answer answer)
    {
        Context.Answers.Remove(answer);
        return Task.CompletedTask;
    }
}
