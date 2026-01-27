using CommunityCar.Application.Common.Interfaces.Repositories.Shared;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Shared;

public class VoteRepository : BaseRepository<Vote>, IVoteRepository
{
    public VoteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Vote?> GetUserVoteAsync(Guid entityId, EntityType entityType, Guid userId)
    {
        return await Context.Votes
            .FirstOrDefaultAsync(v => v.EntityId == entityId && 
                                    v.EntityType == entityType && 
                                    v.UserId == userId);
    }

    public async Task<int> GetVoteCountAsync(Guid entityId, EntityType entityType)
    {
        var upVotes = await Context.Votes
            .CountAsync(v => v.EntityId == entityId && 
                           v.EntityType == entityType && 
                           v.Type == VoteType.Upvote);

        var downVotes = await Context.Votes
            .CountAsync(v => v.EntityId == entityId && 
                           v.EntityType == entityType && 
                           v.Type == VoteType.Downvote);

        return upVotes - downVotes;
    }

    public async Task<int> GetVoteCountAsync(Guid entityId, EntityType entityType, VoteType voteType)
    {
        return await Context.Votes
            .CountAsync(v => v.EntityId == entityId && 
                           v.EntityType == entityType && 
                           v.Type == voteType);
    }

    public async Task<int> GetVoteScoreAsync(Guid entityId, EntityType entityType)
    {
        var upVotes = await Context.Votes
            .CountAsync(v => v.EntityId == entityId && 
                           v.EntityType == entityType && 
                           v.Type == VoteType.Upvote);

        var downVotes = await Context.Votes
            .CountAsync(v => v.EntityId == entityId && 
                           v.EntityType == entityType && 
                           v.Type == VoteType.Downvote);

        return upVotes - downVotes;
    }

    public async Task<IEnumerable<Vote>> GetVotesByEntityAsync(Guid entityId, EntityType entityType)
    {
        return await Context.Votes
            .Where(v => v.EntityId == entityId && v.EntityType == entityType)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vote>> GetUserVotesAsync(Guid userId, EntityType? entityType = null)
    {
        var query = Context.Votes.Where(v => v.UserId == userId);
        
        if (entityType.HasValue)
        {
            query = query.Where(v => v.EntityType == entityType.Value);
        }
        
        return await query.ToListAsync();
    }
}
