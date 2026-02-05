using Microsoft.EntityFrameworkCore;
using CommunityCar.Application.Common.Interfaces.Repositories.Chat;
using CommunityCar.Domain.Entities.Chats;
using CommunityCar.Infrastructure.Persistence.Data;
using CommunityCar.Infrastructure.Persistence.Repositories.Base;

namespace CommunityCar.Web.Areas.Communication.Repositories.Chat;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetConversationMessagesAsync(Guid conversationId, int page = 1, int pageSize = 50)
    {
        return await Context.Messages
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(Guid userId)
    {
        return await Context.Messages
            .Include(m => m.Conversation)
            .ThenInclude(c => c.Participants)
            .Where(m => m.Conversation.Participants.Any(p => p.UserId == userId && !p.IsDeleted))
            .Where(m => m.SenderId != userId && !m.IsDeleted)
            .Where(m => !Context.ConversationParticipants
                .Any(p => p.ConversationId == m.ConversationId && 
                         p.UserId == userId && 
                         p.LastReadAt >= m.CreatedAt))
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadMessageCountAsync(Guid userId)
    {
        return await Context.Messages
            .Include(m => m.Conversation)
            .ThenInclude(c => c.Participants)
            .Where(m => m.Conversation.Participants.Any(p => p.UserId == userId && !p.IsDeleted))
            .Where(m => m.SenderId != userId && !m.IsDeleted)
            .Where(m => !Context.ConversationParticipants
                .Any(p => p.ConversationId == m.ConversationId && 
                         p.UserId == userId && 
                         p.LastReadAt >= m.CreatedAt))
            .CountAsync();
    }

    public async Task<int> GetConversationUnreadCountAsync(Guid conversationId, Guid userId)
    {
        var participant = await Context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant == null) return 0;

        return await Context.Messages
            .Where(m => m.ConversationId == conversationId && 
                       m.SenderId != userId && 
                       !m.IsDeleted &&
                       m.CreatedAt > (participant.LastReadAt ?? DateTime.MinValue))
            .CountAsync();
    }

    public async Task<Message?> GetLastMessageAsync(Guid conversationId)
    {
        return await Context.Messages
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId)
    {
        var message = await Context.Messages
            .Include(m => m.Conversation)
            .ThenInclude(c => c.Participants)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null) return false;

        var participant = message.Conversation.Participants
            .FirstOrDefault(p => p.UserId == userId);

        if (participant == null) return false;

        participant.UpdateLastRead(message.CreatedAt);
        Context.ConversationParticipants.Update(participant);
        return true;
    }

    public async Task<bool> MarkConversationAsReadAsync(Guid conversationId, Guid userId)
    {
        var participant = await Context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant == null) return false;

        participant.UpdateLastRead(DateTime.UtcNow);
        Context.ConversationParticipants.Update(participant);
        return true;
    }

    public async Task<bool> DeleteMessageAsync(Guid messageId, Guid userId)
    {
        var message = await GetByIdAsync(messageId);
        if (message == null || message.SenderId != userId) return false;

        message.SoftDelete(userId.ToString());
        await UpdateAsync(message);
        return true;
    }

    public async Task<bool> EditMessageAsync(Guid messageId, string newContent)
    {
        var message = await GetByIdAsync(messageId);
        if (message == null) return false;

        message.EditContent(newContent);
        await UpdateAsync(message);
        return true;
    }

    public async Task<IEnumerable<Message>> SearchMessagesAsync(Guid userId, string searchTerm, int page = 1, int pageSize = 20)
    {
        return await Context.Messages
            .Include(m => m.Conversation)
            .ThenInclude(c => c.Participants)
            .Where(m => m.Conversation.Participants.Any(p => p.UserId == userId && !p.IsDeleted))
            .Where(m => m.Content.Contains(searchTerm) && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
