using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories.Chat;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;

namespace CommunityCar.Web.Areas.Communication.Repositories.Chat;

public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await Context.Conversations
            .Include(c => c.Participants)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .Where(c => c.Participants.Any(p => p.UserId == userId && !p.IsDeleted))
            .OrderByDescending(c => c.Messages.Max(m => m.CreatedAt))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Conversation?> GetConversationWithParticipantsAsync(Guid conversationId)
    {
        return await Context.Conversations
            .Include(c => c.Participants)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(50))
            .FirstOrDefaultAsync(c => c.Id == conversationId);
    }

    public async Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2)
    {
        return await Context.Conversations
            .Include(c => c.Participants)
            .Where(c => c.Participants.Count == 2)
            .Where(c => c.Participants.Any(p => p.UserId == userId1) && 
                       c.Participants.Any(p => p.UserId == userId2))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsUserParticipantAsync(Guid conversationId, Guid userId)
    {
        return await Context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId && !p.IsDeleted);
    }

    public async Task<bool> AddParticipantAsync(Guid conversationId, Guid userId)
    {
        var existingParticipant = await Context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (existingParticipant != null)
        {
            if (existingParticipant.IsDeleted)
            {
                existingParticipant.Restore(userId.ToString());
                Context.ConversationParticipants.Update(existingParticipant);
                return true;
            }
            return false; // Already a participant
        }

        var participant = ConversationParticipant.Create(conversationId, userId);
        Context.ConversationParticipants.Add(participant);
        return true;
    }

    public async Task<bool> RemoveParticipantAsync(Guid conversationId, Guid userId)
    {
        var participant = await Context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant == null) return false;

        participant.SoftDelete(userId.ToString());
        Context.ConversationParticipants.Update(participant);
        return true;
    }

    public async Task<int> GetParticipantCountAsync(Guid conversationId)
    {
        return await Context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId && !p.IsDeleted)
            .CountAsync();
    }

    public async Task<IEnumerable<Guid>> GetParticipantIdsAsync(Guid conversationId)
    {
        return await Context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId && !p.IsDeleted)
            .Select(p => p.UserId)
            .ToListAsync();
    }

    public async Task<bool> ArchiveConversationAsync(Guid conversationId, Guid userId)
    {
        var participant = await Context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant == null) return false;

        participant.Archive();
        Context.ConversationParticipants.Update(participant);
        return true;
    }

    public async Task<bool> UnarchiveConversationAsync(Guid conversationId, Guid userId)
    {
        var participant = await Context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant == null) return false;

        participant.Unarchive();
        Context.ConversationParticipants.Update(participant);
        return true;
    }

    public async Task<IEnumerable<Conversation>> GetArchivedConversationsAsync(Guid userId)
    {
        return await Context.Conversations
            .Include(c => c.Participants)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .Where(c => c.Participants.Any(p => p.UserId == userId && p.IsArchived && !p.IsDeleted))
            .OrderByDescending(c => c.Messages.Max(m => m.CreatedAt))
            .ToListAsync();
    }
}
