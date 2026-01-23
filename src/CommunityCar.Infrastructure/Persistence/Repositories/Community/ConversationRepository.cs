using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Community;

public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Conversation>> GetUserConversationsAsync(Guid userId)
    {
        return await Context.Set<Conversation>()
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .Include(c => c.Participants)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .OrderByDescending(c => c.Messages.Max(m => m.CreatedAt))
            .ToListAsync();
    }

    public async Task<Conversation?> GetConversationWithParticipantsAsync(Guid conversationId)
    {
        return await Context.Set<Conversation>()
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
    }

    public async Task<bool> IsUserParticipantAsync(Guid conversationId, Guid userId)
    {
        return await Context.Set<ConversationParticipant>()
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);
    }

    public async Task AddParticipantAsync(Guid conversationId, Guid userId)
    {
        var participant = new ConversationParticipant(conversationId, userId);
        await Context.Set<ConversationParticipant>().AddAsync(participant);
    }

    public async Task RemoveParticipantAsync(Guid conversationId, Guid userId)
    {
        var participant = await Context.Set<ConversationParticipant>()
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);
        
        if (participant != null)
        {
            Context.Set<ConversationParticipant>().Remove(participant);
        }
    }
}