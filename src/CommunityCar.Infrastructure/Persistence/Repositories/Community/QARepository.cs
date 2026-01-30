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

    public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(Guid questionId)
    {
        return await Context.Answers
            .Where(a => a.QuestionId == questionId)
            .OrderByDescending(a => a.IsAccepted)
            .ThenByDescending(a => a.VoteScore)
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

    public async Task UpdateAnswerAsync(Answer answer)
    {
        Context.Answers.Update(answer);
        await Task.CompletedTask;
    }

    public async Task<Question?> GetQuestionByIdAsync(Guid id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<Question?> GetQuestionBySlugAsync(string slug)
    {
        return await Context.Questions.FirstOrDefaultAsync(q => q.Slug == slug);
    }
}
